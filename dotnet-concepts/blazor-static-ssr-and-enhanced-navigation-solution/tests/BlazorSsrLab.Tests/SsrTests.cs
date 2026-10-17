using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorSsrLab.Tests;

public class SsrTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    // No auto redirect, so the tests can look at the 302 itself.
    // The client keeps cookies by default (needed for the antiforgery cookie).
    private readonly HttpClient _client = factory.CreateClient(
        new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    // TODO-00
    [Fact]
    public async Task ShouldRenderHomeAsPlainHtmlWithoutInteractivity()
    {
        var response = await _client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("<h1>Home</h1>", html);
        Assert.DoesNotContain("blazor.server.js", html);
        Assert.DoesNotContain("blazor.webassembly.js", html);
    }

    // TODO-01
    [Fact]
    public async Task ShouldLoadBlazorWebScriptFromLayout()
    {
        var html = await _client.GetStringAsync("/");

        Assert.Contains("_framework/blazor.web.js", html);
        Assert.Contains("<base href=\"/\"", html);
    }

    // TODO-02
    [Fact]
    public async Task ShouldBindRouteParameterWithIntConstraint()
    {
        var html = await _client.GetStringAsync("/orders/42");

        Assert.Contains("<h1>Order 42</h1>", html);
    }

    // TODO-02
    [Fact]
    public async Task ShouldNotMatchRouteWhenConstraintFails()
    {
        var response = await _client.GetAsync("/orders/abc");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // TODO-03
    [Fact]
    public async Task ShouldBindQueryStringIntoComponentParameter()
    {
        var html = await _client.GetStringAsync("/search?term=blazor");

        Assert.Contains("Results for: blazor", html);
        Assert.DoesNotContain("Type a term", html);
    }

    // TODO-03
    [Fact]
    public async Task ShouldHtmlEncodeQueryValue()
    {
        var html = await _client.GetStringAsync("/search?term=%3Cb%3E");

        Assert.Contains("&lt;b&gt;", html);
        Assert.DoesNotContain("<b>", html);
    }

    // TODO-04
    [Fact]
    public async Task ShouldRenderEnhancedFormWithHandlerAndAntiforgeryToken()
    {
        var html = await _client.GetStringAsync("/contact");

        Assert.Contains("method=\"post\"", html);
        Assert.Contains("data-enhance", html);
        Assert.Matches("name=\"_handler\"[^>]*value=\"contact\"", html);
        Assert.Contains("__RequestVerificationToken", html);
        Assert.Contains("name=\"Model.Name\"", html);
    }

    // TODO-05
    [Fact]
    public async Task ShouldRerenderWithValidationMessageWhenFormIsInvalid()
    {
        var response = await PostContactAsync("");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Name is required", html);
    }

    // TODO-05
    [Fact]
    public async Task ShouldRedirectAfterValidPost()
    {
        var response = await PostContactAsync("Ana");

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.EndsWith("/thanks?name=Ana", response.Headers.Location?.OriginalString);

        var thanks = await _client.GetStringAsync(response.Headers.Location);
        Assert.Contains("Thanks, Ana", thanks);
    }

    // Bonus TODO-06
    [Trait("Category", "Bonus")]
    [Fact]
    public async Task ShouldRejectPostWithoutAntiforgeryToken()
    {
        await _client.GetAsync("/contact"); // gets the cookie, but we omit the token field
        var response = await _client.PostAsync("/contact", new FormUrlEncodedContent(
            [new("_handler", "contact"), new("Model.Name", "Ana")]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // Fetches the form (token field + cookie), then posts it back like a browser would.
    private async Task<HttpResponseMessage> PostContactAsync(string name)
    {
        var html = await _client.GetStringAsync("/contact");
        var match = Regex.Match(html, "__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "The form must render the antiforgery token.");
        var token = WebUtility.HtmlDecode(match.Groups[1].Value);

        return await _client.PostAsync("/contact", new FormUrlEncodedContent(
        [
            new("_handler", "contact"),
            new("__RequestVerificationToken", token),
            new("Model.Name", name),
        ]));
    }
}
