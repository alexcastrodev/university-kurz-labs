using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StockBatching;

// Reads stock movements from a channel and applies them to the ledger in batches, once per interval.
//
// The rules the tests must protect:
// 1. Nothing is applied before the first tick, and everything queued before a tick is applied on that tick.
// 2. A call to the ledger never carries more than MaxBatchSize movements.
// 3. A batch the ledger rejects is logged and applied again on the next tick; the worker keeps running.
// 4. When the channel is completed, the worker applies what is left and finishes.
// 5. When the host stops, the worker applies what is already queued before it finishes.
public sealed class StockBatchWorker(
    ChannelReader<StockMovement> reader,
    IStockLedger ledger,
    TimeProvider timeProvider,
    StockBatchOptions options,
    ILogger<StockBatchWorker> logger) : BackgroundService
{
    private readonly List<StockMovement> _pending = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(options.Interval, timeProvider);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await FlushAsync(stoppingToken);
                if (reader.Completion.IsCompleted)
                {
                    logger.LogInformation("The stock channel was completed and drained");
                    return;
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // The host is stopping. Fall through and apply what was already queued.
        }

        await FlushAsync(CancellationToken.None);
    }

    private async Task FlushAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            while (_pending.Count < options.MaxBatchSize && reader.TryRead(out var movement))
            {
                _pending.Add(movement);
            }

            if (_pending.Count == 0)
            {
                return;
            }

            try
            {
                await ledger.ApplyAsync(_pending.ToArray(), cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // Keep the batch: it is applied again on the next tick.
                logger.LogError(exception, "Failed to apply {Count} stock movements; retrying on the next tick", _pending.Count);
                return;
            }

            logger.LogInformation("Applied {Count} stock movements", _pending.Count);
            _pending.Clear();
        }
    }
}
