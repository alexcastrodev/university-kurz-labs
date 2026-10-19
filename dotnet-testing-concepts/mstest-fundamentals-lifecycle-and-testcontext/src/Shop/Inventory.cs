namespace Shop;

public sealed class UnknownProductException(string sku)
    : ArgumentException($"There is no product with SKU '{sku}'.", nameof(sku))
{
    public string Sku { get; } = sku;
}

public sealed class OutOfStockException(string sku, int requested, int available)
    : InvalidOperationException($"Cannot reserve {requested} of '{sku}': only {available} left.")
{
    public string Sku { get; } = sku;
    public int Requested { get; } = requested;
    public int Available { get; } = available;
}

// SKUs are case-sensitive: "apple" and "Apple" are different products.
public sealed class Inventory
{
    private readonly Dictionary<string, (int Quantity, decimal UnitPrice)> _products = new(StringComparer.Ordinal);

    public void Restock(string sku, int quantity, decimal unitPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        _products[sku] = (Available(sku) + quantity, unitPrice);
    }

    public int Available(string sku) =>
        _products.TryGetValue(sku, out var product) ? product.Quantity : 0;

    public decimal PriceOf(string sku) =>
        _products.TryGetValue(sku, out var product) ? product.UnitPrice : throw new UnknownProductException(sku);

    public void Reserve(string sku, int quantity)
    {
        if (!_products.TryGetValue(sku, out var product))
        {
            throw new UnknownProductException(sku);
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

        if (product.Quantity < quantity)
        {
            throw new OutOfStockException(sku, quantity, product.Quantity);
        }

        _products[sku] = product with { Quantity = product.Quantity - quantity };
    }

    public void Release(string sku, int quantity)
    {
        var product = _products[sku];
        _products[sku] = product with { Quantity = product.Quantity + quantity };
    }
}
