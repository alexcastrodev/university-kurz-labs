using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthLab.Tests;

public class BonusTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly WebApplicationFactoryClientOptions NoRedirect = new() { AllowAutoRedirect = false };

    [Fact]
    public async Task ShouldRedirectAnonymousToLoginWhenCookieRedirectIsAllowedOnAnApiEndpoint()
    {
        var response = await factory.CreateClient(NoRedirect).GetAsync("/admin-redirect");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task ShouldAuthenticateFromTheTestSchemeWithoutLogin()
    {
        var client = factory.WithWebHostBuilder(b => b.ConfigureTestServices(services =>
            services.AddAuthentication(TestAuthHandler.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { })))
            .CreateClient(NoRedirect);

        var anonymous = await client.GetAsync("/admin");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);

        client.DefaultRequestHeaders.Add(TestAuthHandler.Header, "ana");
        var admin = await client.GetAsync("/admin");
        Assert.Equal(HttpStatusCode.OK, admin.StatusCode);
        var doc = await client.GetAsync("/doc/bob");
        Assert.Equal(HttpStatusCode.Forbidden, doc.StatusCode);
    }

    private sealed class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string Scheme = "Test";
        public const string Header = "X-Test-User";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(Header, out var name))
                return Task.FromResult(AuthenticateResult.NoResult());

            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Name, name.ToString()), new Claim(ClaimTypes.Role, "admin")], Scheme);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
