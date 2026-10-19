using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Notes.Api.Tests;

public sealed class NotesApiFactory(bool useFakeAuthentication = true) : WebApplicationFactory<Program>
{
    // Test-only key: 32 bytes, as HS256 requires at least 256 bits.
    public const string SigningKey = "test-signing-key-0123456789abcdef";
    public const string Issuer = "https://identity.notes.example";
    public const string Audience = "notes-api";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Auth:SigningKey", SigningKey);

        if (useFakeAuthentication)
        {
            // Registered after Program.cs, so "Test" replaces JWT bearer as the default scheme.
            builder.ConfigureTestServices(services =>
                services.AddAuthentication(TestAuthHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { }));
        }
    }

    public HttpClient Anonymous() => CreateClient();

    public HttpClient ClientAs(string userId, params string[] roles)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, userId);
        if (roles.Length > 0)
        {
            client.DefaultRequestHeaders.Add(TestAuthHandler.RolesHeader, string.Join(',', roles));
        }

        return client;
    }
}
