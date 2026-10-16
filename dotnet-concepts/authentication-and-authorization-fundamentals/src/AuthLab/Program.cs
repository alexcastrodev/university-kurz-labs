using System.Security.Claims;
using AuthLab;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// TODO-00: Register authentication with the cookie scheme as the default scheme.

// TODO-01: Register MinimumAgeHandler and OwnerHandler as singleton IAuthorizationHandler services.

// TODO-02: Use AddAuthorizationBuilder and set a fallback policy that requires an authenticated user,
// so an endpoint without any authorization metadata is still closed.

// TODO-03: Add the named policies: "AdminOnly" (role admin), "Sales" (claim department = sales)
// and "Adult" (MinimumAgeRequirement of 18).

var app = builder.Build();

// DEMO ONLY: signs in whoever asks, without a password. It exists to produce users for the tests
// and must never ship.
app.MapPost("/login", async (HttpContext ctx, string name, string? role, string? dept, int? age) =>
{
    var claims = new List<Claim> { new(ClaimTypes.Name, name) };
    if (role is not null) claims.Add(new(ClaimTypes.Role, role));
    if (dept is not null) claims.Add(new("department", dept));
    if (age is not null) claims.Add(new("age", age.Value.ToString()));

    // TODO-04: An identity only counts as authenticated when it has an authentication type.
    // Create it with the cookie scheme name and sign the principal in.
    await Task.CompletedTask;
    return Results.Ok();
}).AllowAnonymous();

// TODO-05: Open this endpoint to anonymous callers.
app.MapGet("/public", () => "anyone");

// No metadata on purpose: only the fallback policy can protect it.
app.MapGet("/fallback", () => "covered by the fallback policy");

// TODO-06: Require an authenticated user on /text and /me (the default policy, no name).
app.MapGet("/text", () => "plain text");
app.MapGet("/me", (ClaimsPrincipal user) => new
{
    user.Identity!.Name,
    IsAdmin = user.IsInRole("admin"),
    Claims = user.Claims.Select(c => $"{c.Type}={c.Value}"),
});

// TODO-07: Same as /text, but a challenge must answer 401 instead of redirecting to the login page.
app.MapGet("/text-401", () => "plain text");

// TODO-08: Protect each endpoint with the policy that matches its name.
app.MapGet("/admin", () => new { ok = "admin" });
app.MapGet("/sales", () => new { ok = "sales" });
app.MapGet("/adult", () => new { ok = "adult" });

app.MapGet("/doc/{owner}", async Task<Results<Ok<string>, ForbidHttpResult>>
    (string owner, ClaimsPrincipal user, IAuthorizationService authz) =>
{
    var result = await authz.AuthorizeAsync(user, owner, new OwnerRequirement());
    return result.Succeeded ? TypedResults.Ok(owner) : TypedResults.Forbid();
}).RequireAuthorization();

app.Run();

public partial class Program;
