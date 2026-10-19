using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace StockBatching;

public static class StockBatchingServiceCollectionExtensions
{
    // Registers the channel (reader and writer), the options and the worker. The application registers
    // its IStockLedger. A TimeProvider registered before this call wins over TimeProvider.System.
    public static IServiceCollection AddStockBatching(this IServiceCollection services, StockBatchOptions options)
    {
        var channel = Channel.CreateBounded<StockMovement>(new BoundedChannelOptions(options.Capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
        });

        services.AddSingleton(options);
        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);
        services.TryAddSingleton(TimeProvider.System);
        services.AddHostedService<StockBatchWorker>();
        return services;
    }
}
