using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Polly.Timeout;

namespace ResilienceLab.Tests;

public sealed class BonusTests
{
    [Fact]
    public async Task ShouldHedgeASlowFirstAttemptWithASecondOne()
    {
        var handler = new ScriptedHandler(async (attempt, ct) =>
        {
            if (attempt == 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var services = new ServiceCollection();
        services.AddPricingClient().ConfigurePrimaryHttpMessageHandler(() => handler);
        var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(ResilienceClients.Pricing);

        var started = DateTime.UtcNow;
        var response = await client.GetAsync("/price/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, handler.Hits);
        Assert.True(DateTime.UtcNow - started < TimeSpan.FromSeconds(2), "the fast second attempt should win");
    }

    [Fact]
    public async Task ShouldLetTheOuterTimeoutCapTheWholeRetrySequence()
    {
        var handler = ScriptedHandler.Hanging();
        var services = new ServiceCollection();
        services.AddStrictClient().ConfigurePrimaryHttpMessageHandler(() => handler);
        var client = services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(ResilienceClients.Strict);

        var started = DateTime.UtcNow;
        var ex = await Assert.ThrowsAsync<TimeoutRejectedException>(() => client.GetAsync("/x").WaitAsync(TimeSpan.FromSeconds(4)));

        Assert.Equal(TimeSpan.FromSeconds(1), ex.Timeout);
        Assert.InRange(handler.Hits, 2, 3);
        Assert.True(DateTime.UtcNow - started < TimeSpan.FromSeconds(2));
    }
}
