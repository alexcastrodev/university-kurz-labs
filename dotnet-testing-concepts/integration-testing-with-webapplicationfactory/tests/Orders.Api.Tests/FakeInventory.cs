using Orders.Api.Orders;

namespace Orders.Api.Tests;

// TODO-00: A fake is a working in-memory implementation, not a script of expected calls. Keep the stock
// per SKU in a ConcurrentDictionary<string, int>. A SKU nobody configured has DefaultStock units, so most
// tests do not have to set anything. SetStock overrides one SKU, Clear forgets every override.
public sealed class FakeInventory : IInventoryClient
{
    public const int DefaultStock = 100;

    public void SetStock(string sku, int available) =>
        throw new NotImplementedException("TODO-00");

    public void Clear() =>
        throw new NotImplementedException("TODO-00");

    public Task<int> GetAvailableAsync(string sku, CancellationToken ct) =>
        throw new NotImplementedException("TODO-00");
}
