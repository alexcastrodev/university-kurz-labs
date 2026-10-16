using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace ResilienceLab.Tests;

public sealed class ResilienceTests
{
    private static HttpClient Client(string name, Action<ServiceCollection, ScriptedHandler> register, ScriptedHandler handler)
    {
        var services = new ServiceCollection();
        register(services, handler);
        return services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(name);
    }

    private static HttpClient Orders(ScriptedHandler handler) =>
        Client(ResilienceClients.Orders, (s, h) => s.AddOrdersClient().ConfigurePrimaryHttpMessageHandler(() => h), handler);

    private static HttpClient Catalog(ScriptedHandler handler) =>
        Client(ResilienceClients.Catalog, (s, h) => s.AddCatalogClient().ConfigurePrimaryHttpMessageHandler(() => h), handler);

    [Fact]
    public async Task ShouldRetryAGetUntilTheServerRecovers()
    {
        var handler = ScriptedHandler.FailTimesThenOk(failures: 2);

        var response = await Orders(handler).GetAsync("/orders/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(3, handler.Hits);
    }

    [Fact]
    public async Task ShouldReturnTheLastResponseWhenRetriesAreExhausted()
    {
        var handler = ScriptedHandler.Always(HttpStatusCode.ServiceUnavailable);

        var response = await Orders(handler).GetAsync("/orders/1");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(3, handler.Hits);
    }

    [Fact]
    public async Task ShouldNeverRetryAPost()
    {
        var handler = ScriptedHandler.FailTimesThenOk(failures: 1);

        var response = await Orders(handler).PostAsync("/orders", new StringContent("{}"));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal(1, handler.Hits);
    }

    [Fact]
    public async Task ShouldTimeEachAttemptOutAndRetryIt()
    {
        var handler = ScriptedHandler.Hanging();

        await Assert.ThrowsAsync<TimeoutRejectedException>(() => Orders(handler).GetAsync("/orders/1").WaitAsync(TimeSpan.FromSeconds(4)));

        Assert.Equal(3, handler.Hits);
    }

    [Fact]
    public async Task ShouldOpenTheCircuitAndFailFastWithoutCallingTheServer()
    {
        var handler = ScriptedHandler.Always(HttpStatusCode.ServiceUnavailable);
        var client = Catalog(handler);

        for (var i = 1; i <= 5; i++)
        {
            Assert.Equal(HttpStatusCode.ServiceUnavailable, (await client.GetAsync("/items")).StatusCode);
        }

        for (var i = 6; i <= 8; i++)
        {
            await Assert.ThrowsAsync<BrokenCircuitException>(() => client.GetAsync("/items"));
        }

        Assert.Equal(5, handler.Hits);
    }

    [Fact]
    public async Task ShouldCloseTheCircuitAfterAProbeSucceeds()
    {
        var handler = ScriptedHandler.FailTimesThenOk(failures: 5);
        var client = Catalog(handler);
        for (var i = 0; i < 5; i++)
        {
            await client.GetAsync("/items");
        }

        await Assert.ThrowsAsync<BrokenCircuitException>(() => client.GetAsync("/items"));
        await Task.Delay(TimeSpan.FromMilliseconds(1300));

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/items")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/items")).StatusCode);
    }
}
