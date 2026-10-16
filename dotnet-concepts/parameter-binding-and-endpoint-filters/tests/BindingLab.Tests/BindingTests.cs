using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BindingLab.Tests;

public class BindingTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ShouldAnswerEveryEndpointWhenAppIsHealthy()
    {
        // One bad handler signature breaks every endpoint, so this is a smoke test.
        var response = await _client.GetAsync("/time");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.InRange(int.Parse(await response.Content.ReadAsStringAsync()), 2025, 2100);
    }

    [Fact]
    public async Task ShouldParsePointFromQueryUsingInvariantCulture()
    {
        var response = await _client.GetAsync("/map?p=12.3,10.1");

        Assert.Equal("Point: 12.3, 10.1", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/map")]
    [InlineData("/map?p=12.3")]
    [InlineData("/map?p=a,b")]
    public async Task ShouldReturn400WhenPointCannotBeParsed(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ShouldBindCallerFromHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/who");
        request.Headers.Add("X-Caller", "alice");

        var response = await _client.SendAsync(request);

        Assert.Equal("caller=alice", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldBindCallerFromQueryWhenHeaderIsMissing()
    {
        var response = await _client.GetAsync("/who?caller=bob");

        Assert.Equal("caller=bob", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldReturn400WhenCallerIsMissing()
    {
        var response = await _client.GetAsync("/who");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ShouldBindSearchRequestFromRouteQueryAndHeader()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/search/books?page=3");
        request.Headers.Add("X-Tenant", "acme");

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("books", body.GetProperty("category").GetString());
        Assert.Equal(3, body.GetProperty("page").GetInt32());
        Assert.Equal("acme", body.GetProperty("tenant").GetString());
    }

    [Fact]
    public async Task ShouldReturn400WhenTenantHeaderIsMissing()
    {
        var response = await _client.GetAsync("/search/books?page=3");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ShouldTrimOrderNameInFilterBeforeHandlerRuns()
    {
        var response = await _client.PostAsJsonAsync("/orders", new { name = "  Zelda  ", quantity = 2 });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("Zelda", body.GetProperty("name").GetString());
    }

    [Theory]
    [InlineData("", 1, "name")]
    [InlineData("Zelda", 0, "quantity")]
    public async Task ShouldRejectInvalidOrderWith400FromFilter(string name, int quantity, string field)
    {
        var response = await _client.PostAsJsonAsync("/orders", new { name, quantity });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.GetProperty("errors").TryGetProperty(field, out _));
    }
}
