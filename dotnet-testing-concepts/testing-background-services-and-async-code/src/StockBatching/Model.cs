namespace StockBatching;

// One change to the stock of a product: +5 when goods arrive, -2 when an order ships.
public sealed record StockMovement(string Sku, int Quantity);

// Where batches are applied (a database, another service). The worker only knows this contract.
public interface IStockLedger
{
    Task ApplyAsync(IReadOnlyList<StockMovement> batch, CancellationToken cancellationToken);
}

public sealed class StockBatchOptions
{
    // How often the worker wakes up and applies what is queued.
    public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(5);

    // The ledger accepts at most this many movements per call.
    public int MaxBatchSize { get; init; } = 100;

    // Producers wait when this many movements are queued and not yet applied.
    public int Capacity { get; init; } = 10_000;
}
