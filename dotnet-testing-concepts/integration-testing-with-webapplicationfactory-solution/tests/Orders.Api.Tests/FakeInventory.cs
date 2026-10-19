using System.Collections.Concurrent;
using Orders.Api.Orders;

namespace Orders.Api.Tests;

// A working in-memory inventory. Every SKU has plenty of stock unless a test says otherwise.
public sealed class FakeInventory : IInventoryClient
{
    public const int DefaultStock = 100;

    private readonly ConcurrentDictionary<string, int> _stock = new();

    public void SetStock(string sku, int available) => _stock[sku] = available;

    public void Clear() => _stock.Clear();

    public Task<int> GetAvailableAsync(string sku, CancellationToken ct) =>
        Task.FromResult(_stock.GetValueOrDefault(sku, DefaultStock));
}
