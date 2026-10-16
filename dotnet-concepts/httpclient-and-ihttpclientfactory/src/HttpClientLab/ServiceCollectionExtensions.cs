using Microsoft.Extensions.DependencyInjection;

namespace HttpClientLab;

public static class ServiceCollectionExtensions
{
    public static readonly TimeSpan ConnectionLifetime = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan HandlerLifetime = TimeSpan.FromMinutes(5);

    public static IHttpClientBuilder AddGamesClient(this IServiceCollection services, Uri baseAddress, TimeSpan? timeout = null)
    {
        // TODO-00: Set BaseAddress and Timeout (default 30 seconds) on the typed client.
        // TODO-01: Register CorrelationIdHandler (transient) and add it with AddHttpMessageHandler.
        // TODO-02: Use a SocketsHttpHandler as primary handler whose PooledConnectionLifetime
        //          is ConnectionLifetime, so a DNS change is eventually noticed.
        // TODO-03: Rotate the handler every HandlerLifetime.
        return services.AddHttpClient<GamesClient>();
    }
}
