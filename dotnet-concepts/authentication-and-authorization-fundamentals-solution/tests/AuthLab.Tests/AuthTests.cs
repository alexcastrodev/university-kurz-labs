using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AuthLab.Tests;

public class AuthTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient NewClient() => factory
        .WithWebHostBuilder(b => b.UseEnvironment("Production"))
        .CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private async Task<HttpClient> SignedIn(string query)
    {
        var client = NewClient();
        var login = await client.PostAsync($"/login?{query}", content: null);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        return client;
    }

    private Task<HttpClient> Ana() => SignedIn("name=ana&role=admin&dept=sales&age=30");

    private Task<HttpClient> Bob() => SignedIn("name=bob&age=16");

    [Fact]
    public async Task ShouldServePublicEndpointToAnonymous()
    {
        var response = await NewClient().GetAsync("/public");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShouldRedirectAnonymousToLoginWhenOnlyTheFallbackPolicyProtects()
    {
        var response = await NewClient().GetAsync("/fallback");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task ShouldRedirectAnonymousForPlainTextEndpoint()
    {
        var response = await NewClient().GetAsync("/text");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturn401ForAnonymousOnJsonEndpoint()
    {
        var response = await NewClient().GetAsync("/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturn401WhenRedirectIsDisabledOnTheEndpoint()
    {
        var response = await NewClient().GetAsync("/text-401");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ShouldExposeTheClaimsOfTheSignedInUser()
    {
        var client = await Ana();

        var response = await client.GetAsync("/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"name\":\"ana\"", body);
        Assert.Contains("\"isAdmin\":true", body);
        Assert.Contains("department=sales", body);
        Assert.Contains("age=30", body);
    }

    [Fact]
    public async Task ShouldAllowAdminRoleOnlyOnAdminEndpoint()
    {
        Assert.Equal(HttpStatusCode.OK, (await (await Ana()).GetAsync("/admin")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await Bob()).GetAsync("/admin")).StatusCode);
    }

    [Fact]
    public async Task ShouldAllowSalesClaimOnlyOnSalesEndpoint()
    {
        Assert.Equal(HttpStatusCode.OK, (await (await Ana()).GetAsync("/sales")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await Bob()).GetAsync("/sales")).StatusCode);
    }

    [Fact]
    public async Task ShouldCheckTheMinimumAgeRequirement()
    {
        Assert.Equal(HttpStatusCode.OK, (await (await Ana()).GetAsync("/adult")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await Bob()).GetAsync("/adult")).StatusCode);
    }

    [Fact]
    public async Task ShouldRejectACallerWithoutTheAgeClaim()
    {
        var client = await SignedIn("name=carol");

        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/adult")).StatusCode);
    }

    [Fact]
    public async Task ShouldAllowOnlyTheOwnerToReadTheirDocument()
    {
        Assert.Equal(HttpStatusCode.OK, (await (await Ana()).GetAsync("/doc/ana")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await (await Bob()).GetAsync("/doc/ana")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await NewClient().GetAsync("/doc/ana")).StatusCode);
    }
}
