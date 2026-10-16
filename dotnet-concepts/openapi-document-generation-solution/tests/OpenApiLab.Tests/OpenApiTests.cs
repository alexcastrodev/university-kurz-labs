using System.Net;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace OpenApiLab.Tests;

public class OpenApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory
        .WithWebHostBuilder(b => b.UseEnvironment("Development"))
        .CreateClient();

    private async Task<JsonNode> GetDocumentAsync(string name)
    {
        var response = await _client.GetAsync($"/openapi/{name}.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return JsonNode.Parse(await response.Content.ReadAsStringAsync())!;
    }

    [Fact]
    public async Task ShouldServeV1DocumentWithOpenApiVersionAndPaths()
    {
        var doc = await GetDocumentAsync("v1");

        Assert.StartsWith("3.", doc["openapi"]!.GetValue<string>());
        Assert.NotNull(doc["paths"]!["/games"]);
        Assert.NotNull(doc["paths"]!["/games/{id}"]);
    }

    [Fact]
    public async Task ShouldDescribeGetGameWithMetadata()
    {
        var doc = await GetDocumentAsync("v1");
        var op = doc["paths"]!["/games/{id}"]!["get"]!;

        Assert.Equal("Gets a game by id", op["summary"]!.GetValue<string>());
        Assert.Equal("GetGame", op["operationId"]!.GetValue<string>());
        Assert.Equal("Games", op["tags"]![0]!.GetValue<string>());
        Assert.NotNull(op["responses"]!["200"]!["content"]!["application/json"]);
        Assert.NotNull(op["responses"]!["404"]);
    }

    [Fact]
    public async Task ShouldSetTitleVersionAndServerWithDocumentTransformer()
    {
        var doc = await GetDocumentAsync("v1");

        Assert.Equal("Game Store API", doc["info"]!["title"]!.GetValue<string>());
        Assert.Equal("2.0.0", doc["info"]!["version"]!.GetValue<string>());
        Assert.Equal("https://api.example.com", doc["servers"]![0]!["url"]!.GetValue<string>());
    }

    [Fact]
    public async Task ShouldMarkOnlyLegacyEndpointAsDeprecated()
    {
        var doc = await GetDocumentAsync("v1");

        Assert.True(doc["paths"]!["/games/legacy"]!["get"]!["deprecated"]!.GetValue<bool>());
        Assert.Null(doc["paths"]!["/games"]!["get"]!["deprecated"]);
    }

    [Fact]
    public async Task ShouldHideInternalEndpointFromDocument()
    {
        var doc = await GetDocumentAsync("v1");

        Assert.Null(doc["paths"]!["/internal/health"]);
        // The endpoint itself still works.
        var response = await _client.GetAsync("/internal/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ShouldPutAdminEndpointOnlyInAdminDocument()
    {
        var admin = await GetDocumentAsync("admin");
        var v1 = await GetDocumentAsync("v1");

        Assert.NotNull(admin["paths"]!["/admin/stats"]);
        Assert.Null(v1["paths"]!["/admin/stats"]);
        // Endpoints without a group name show up in both documents.
        Assert.NotNull(admin["paths"]!["/games"]);
    }

    [Fact]
    public async Task ShouldDescribeEveryDecimalWithSchemaTransformer_Bonus()
    {
        var doc = await GetDocumentAsync("v1");
        var price = doc["components"]!["schemas"]!["GameDto"]!["properties"]!["price"]!;

        Assert.Equal("Money in euros", price["description"]!.GetValue<string>());
    }
}
