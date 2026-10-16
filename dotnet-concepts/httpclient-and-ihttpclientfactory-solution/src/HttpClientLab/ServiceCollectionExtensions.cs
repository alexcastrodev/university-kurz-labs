using Microsoft.Extensions.DependencyInjection;

namespace HttpClientLab;

public static class ServiceCollectionExtensions
{
    public static readonly TimeSpan ConnectionLifetime = TimeSpan.FromMinutes(2);
    public static readonly TimeSpan HandlerLifetime = TimeSpan.FromMinutes(5);

    public static IHttpClientBuilder AddGamesClient(this IServiceCollection services, Uri baseAddress, TimeSpan? timeout = null)
    {
        services.AddTransient<CorrelationIdHandler>();

        return services
            .AddHttpClient<GamesClient>(client =>
            {
                client.BaseAddress = baseAddress;
                client.Timeout = timeout ?? TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<CorrelationIdHandler>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionLifetime = ConnectionLifetime })
            .SetHandlerLifetime(HandlerLifetime);
    }
}
