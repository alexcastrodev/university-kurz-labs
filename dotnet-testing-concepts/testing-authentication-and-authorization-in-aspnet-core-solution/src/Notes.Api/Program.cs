using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notes.Api.Notes;

var builder = WebApplication.CreateBuilder(args);

// The signing key never lives in appsettings.json: it comes from user secrets, the environment or a vault.
var auth = builder.Configuration.GetSection("Auth");
var signingKey = auth["SigningKey"]
    ?? throw new InvalidOperationException("Auth:SigningKey is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = auth["Issuer"],
        ValidAudience = auth["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
    });
builder.Services.AddNotesAuthorization();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<NoteStore>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("healthy")).AllowAnonymous();
app.MapNotesEndpoints();
app.MapAdminEndpoints();

app.Run();
