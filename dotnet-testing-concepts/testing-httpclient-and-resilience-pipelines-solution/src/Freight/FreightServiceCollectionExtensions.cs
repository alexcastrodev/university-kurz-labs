using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace Freight;

public static class FreightServiceCollectionExtensions
{
    // The pipeline resolves TimeProvider from the container when one is registered, so a test that
    // registers a FakeTimeProvider controls every retry delay, timeout and break duration.
    public static IHttpClientBuilder AddFreightClient(this IServiceCollection services, Uri baseAddress)
    {
        var builder = services.AddHttpClient<FreightClient>(client => client.BaseAddress = baseAddress);

        // Handlers run in the order they are added, first one outermost. The key handler goes first so it
        // runs once per call and every retry carries the same key.
        builder.AddHttpMessageHandler(() => new IdempotencyKeyHandler());
        builder.AddResilienceHandler("freight", pipeline => pipeline
            .AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromSeconds(1),
            })
            .AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                MinimumThroughput = 4,
                SamplingDuration = TimeSpan.FromSeconds(30),
                BreakDuration = TimeSpan.FromSeconds(30),
            })
            .AddTimeout(TimeSpan.FromSeconds(2)));

        return builder;
    }
}
