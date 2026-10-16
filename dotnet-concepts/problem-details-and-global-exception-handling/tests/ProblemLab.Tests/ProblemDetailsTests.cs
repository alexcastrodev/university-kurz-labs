using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ProblemLab.Tests;

public class ProblemDetailsTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory
        .WithWebHostBuilder(b => b.UseEnvironment("Production"))
        .CreateClient();

    private static void AssertProblemJson(HttpResponseMessage response) =>
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

    [Fact]
    public async Task ShouldReturn200WhenGameExists()
    {
        var response = await _client.GetAsync("/games/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShouldReturn404ProblemWhenGameIsMissing()
    {
        var response = await _client.GetAsync("/games/42");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertProblemJson(response);
        Assert.Equal("Game not found", body.GetProperty("title").GetString());
        Assert.Equal("Game 42 not found", body.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task ShouldReturnProblemWhenRouteDoesNotExist()
    {
        var response = await _client.GetAsync("/nope");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        AssertProblemJson(response);
    }

    [Fact]
    public async Task ShouldReturn503WithRetryAfterWhenDependencyTimesOut()
    {
        var response = await _client.GetAsync("/timeout");
        var raw = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        AssertProblemJson(response);
        Assert.Equal(5, response.Headers.RetryAfter?.Delta?.TotalSeconds);
        Assert.DoesNotContain("db.internal", raw);
    }

    [Fact]
    public async Task ShouldReturnGeneric500WithoutLeakingExceptionMessage()
    {
        var response = await _client.GetAsync("/boom");
        var raw = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        AssertProblemJson(response);
        Assert.DoesNotContain("secret123", raw);
    }

    [Theory]
    [InlineData("/games/42")]
    [InlineData("/nope")]
    [InlineData("/boom")]
    public async Task ShouldSetInstanceToRequestPathOnEveryProblem(string path)
    {
        var response = await _client.GetAsync(path);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(path, body.GetProperty("instance").GetString());
    }
}
