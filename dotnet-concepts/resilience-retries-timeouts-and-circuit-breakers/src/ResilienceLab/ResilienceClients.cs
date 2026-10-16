using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

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
        // TODO-00: Add the standard resilience handler and configure it with ConfigureOrders.
        return builder;
    }

    public static void ConfigureOrders(HttpStandardResilienceOptions options)
    {
        // TODO-02: Allow 2 retries with a 10 ms base delay (tests must stay fast).
        // TODO-03: Never retry unsafe HTTP methods such as POST.
        // TODO-04: Attempt timeout 300 ms, total timeout 5 s. Validation rule: the circuit
        //          breaker SamplingDuration must be at least twice the attempt timeout, so
        //          set it to 10 s as well.
    }

    public static IHttpClientBuilder AddCatalogClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Catalog, c => c.BaseAddress = new Uri("http://catalog.test"));
        // TODO-01: Add the standard resilience handler and configure it with ConfigureCatalog.
        return builder;
    }

    public static void ConfigureCatalog(HttpStandardResilienceOptions options)
    {
        // TODO-05: Stop retrying GET (MaxRetryAttempts cannot be 0, find the method that
        //          disables retries for a verb), so the breaker sees every failure once.
        // TODO-06: Attempt timeout 1 s, total timeout 5 s, and a circuit breaker that opens
        //          after 5 calls when at least half failed within 10 s, staying open 1 s.
        //          The default MinimumThroughput is 100, which is why it never opens in tests.
    }

    public static IHttpClientBuilder AddPricingClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Pricing, c => c.BaseAddress = new Uri("http://pricing.test"));
        // TODO-07 (optional): Use the standard hedging handler instead of retries: send a
        //          second request after 100 ms of silence, at most one extra attempt.
        return builder;
    }

    public static IHttpClientBuilder AddStrictClient(this IServiceCollection services)
    {
        var builder = services.AddHttpClient(Strict, c => c.BaseAddress = new Uri("http://strict.test"));
        // TODO-08 (optional): Build a custom pipeline named "strict" with AddResilienceHandler:
        //          outer timeout 1 s, then a retry (3 attempts, 50 ms), then an inner timeout
        //          of 300 ms. Order matters: the first strategy added is the outermost.
        return builder;
    }
}
