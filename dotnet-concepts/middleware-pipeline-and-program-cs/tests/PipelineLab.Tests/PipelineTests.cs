using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PipelineLab.Tests;

public class PipelineTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private HttpClient MaintenanceClient() => factory
        .WithWebHostBuilder(b => b.UseSetting("Maintenance:Enabled", "true"))
        .CreateClient();

    [Fact]
    public async Task ShouldReachEndpointWhenNothingShortCircuits()
    {
        var response = await _client.GetAsync("/hello");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShouldGenerateCorrelationIdWhenRequestHasNone()
    {
        var response = await _client.GetAsync("/hello");

        var id = Assert.Single(response.Headers.GetValues("X-Correlation-Id"));
        Assert.Equal(32, id.Length);
        Assert.Equal($"hello {id}", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldEchoIncomingCorrelationId()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/hello");
        request.Headers.Add("X-Correlation-Id", "abc-123");

        var response = await _client.SendAsync(request);

        Assert.Equal("abc-123", Assert.Single(response.Headers.GetValues("X-Correlation-Id")));
        Assert.Equal("hello abc-123", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldTurnEndpointExceptionIntoControlled500()
    {
        var response = await _client.GetAsync("/boom");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("caught: kaboom", await response.Content.ReadAsStringAsync());
        Assert.True(response.Headers.Contains("X-Correlation-Id"));
    }

    [Fact]
    public async Task ShouldRewritePathAndPathBaseInsideMapBranch()
    {
        var response = await _client.GetAsync("/branch/x");

        Assert.Equal("pathBase=/branch path=/x", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldShortCircuitWith503WhenMaintenanceIsOn()
    {
        var response = await MaintenanceClient().GetAsync("/hello");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("maintenance", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task ShouldKeepCorrelationIdOnShortCircuitedResponse()
    {
        var response = await MaintenanceClient().GetAsync("/hello");

        Assert.True(response.Headers.Contains("X-Correlation-Id"));
    }

    [Fact]
    public async Task ShouldLetHealthEndpointThroughDuringMaintenance()
    {
        var response = await MaintenanceClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
