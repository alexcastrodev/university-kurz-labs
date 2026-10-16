using System.Security.Claims;
using AuthLab;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, OwnerHandler>();
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
    .AddPolicy("AdminOnly", p => p.RequireRole("admin"))
    .AddPolicy("Sales", p => p.RequireClaim("department", "sales"))
    .AddPolicy("Adult", p => p.AddRequirements(new MinimumAgeRequirement(18)));

var app = builder.Build();

// DEMO ONLY: signs in whoever asks, without a password. It exists to produce users for the tests
// and must never ship.
app.MapPost("/login", async (HttpContext ctx, string name, string? role, string? dept, int? age) =>
{
    var claims = new List<Claim> { new(ClaimTypes.Name, name) };
    if (role is not null) claims.Add(new(ClaimTypes.Role, role));
    if (dept is not null) claims.Add(new("department", dept));
    if (age is not null) claims.Add(new("age", age.Value.ToString()));

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await ctx.SignInAsync(new ClaimsPrincipal(identity));
    return Results.Ok();
}).AllowAnonymous();

app.MapGet("/public", () => "anyone").AllowAnonymous();

// No metadata on purpose: only the fallback policy can protect it.
app.MapGet("/fallback", () => "covered by the fallback policy");

app.MapGet("/text", () => "plain text").RequireAuthorization();
app.MapGet("/me", (ClaimsPrincipal user) => new
{
    user.Identity!.Name,
    IsAdmin = user.IsInRole("admin"),
    Claims = user.Claims.Select(c => $"{c.Type}={c.Value}"),
}).RequireAuthorization();

app.MapGet("/text-401", () => "plain text").RequireAuthorization().DisableCookieRedirect();

app.MapGet("/admin", () => new { ok = "admin" }).RequireAuthorization("AdminOnly");
app.MapGet("/sales", () => new { ok = "sales" }).RequireAuthorization("Sales");
app.MapGet("/adult", () => new { ok = "adult" }).RequireAuthorization("Adult");

app.MapGet("/admin-redirect", () => new { ok = "admin" }).RequireAuthorization("AdminOnly").AllowCookieRedirect();

app.MapGet("/doc/{owner}", async Task<Results<Ok<string>, ForbidHttpResult>>
    (string owner, ClaimsPrincipal user, IAuthorizationService authz) =>
{
    var result = await authz.AuthorizeAsync(user, owner, new OwnerRequirement());
    return result.Succeeded ? TypedResults.Ok(owner) : TypedResults.Forbid();
}).RequireAuthorization();

app.Run();

public partial class Program;
