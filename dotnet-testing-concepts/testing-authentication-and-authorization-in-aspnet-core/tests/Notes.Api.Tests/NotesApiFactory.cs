using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Notes.Api.Tests;

public sealed class NotesApiFactory(bool useFakeAuthentication = true) : WebApplicationFactory<Program>
{
    // Test-only key: 32 bytes, as HS256 requires at least 256 bits.
    public const string SigningKey = "test-signing-key-0123456789abcdef";
    public const string Issuer = "https://identity.notes.example";
    public const string Audience = "notes-api";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs refuses to start without a signing key, and reads it before Build: UseSetting reaches it.
        builder.UseSetting("Auth:SigningKey", SigningKey);

        if (useFakeAuthentication)
        {
            // TODO-01: Inside builder.ConfigureTestServices(services => ...), register TestAuthHandler as a
            // scheme named TestAuthHandler.SchemeName and make it the default scheme:
            // services.AddAuthentication(defaultScheme).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(...).
            // Program.cs made JWT bearer the default; ConfigureTestServices runs after it, so yours wins.
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
