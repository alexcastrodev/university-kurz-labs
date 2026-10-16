using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace ResilienceLab;

public static class ResilienceClients
{
    public const string Orders = "orders";
    public const string Catalog = "catalog";
    public const string Pricing = "pricing";
    public const string Strict = "strict";

    public static IHttpClientBuilder AddOrdersClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Orders, c => c.BaseAddress = new Uri("http://orders.test"));
        builder.AddStandardResilienceHandler(ConfigureOrders);
        return builder;
    }

    public static void ConfigureOrders(HttpStandardResilienceOptions options)
    {
        options.Retry.MaxRetryAttempts = 2;
        options.Retry.Delay = TimeSpan.FromMilliseconds(10);
        options.Retry.DisableForUnsafeHttpMethods();
        options.AttemptTimeout.Timeout = TimeSpan.FromMilliseconds(300);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(5);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
    }

    public static IHttpClientBuilder AddCatalogClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Catalog, c => c.BaseAddress = new Uri("http://catalog.test"));
        builder.AddStandardResilienceHandler(ConfigureCatalog);
        return builder;
    }

    public static void ConfigureCatalog(HttpStandardResilienceOptions options)
    {
        options.Retry.DisableFor(HttpMethod.Get);
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(1);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(5);
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(1);
    }

    public static IHttpClientBuilder AddPricingClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Pricing, c => c.BaseAddress = new Uri("http://pricing.test"));
        builder.AddStandardHedgingHandler().Configure(options =>
        {
            options.Hedging.Delay = TimeSpan.FromMilliseconds(100);
            options.Hedging.MaxHedgedAttempts = 1;
        });
        return builder;
    }

    public static IHttpClientBuilder AddStrictClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Strict, c => c.BaseAddress = new Uri("http://strict.test"));
        builder.AddResilienceHandler("strict", pipeline => pipeline
            .AddTimeout(TimeSpan.FromSeconds(1))
            .AddRetry(new HttpRetryStrategyOptions { MaxRetryAttempts = 3, Delay = TimeSpan.FromMilliseconds(50) })
            .AddTimeout(TimeSpan.FromMilliseconds(300)));
        return builder;
    }
}
