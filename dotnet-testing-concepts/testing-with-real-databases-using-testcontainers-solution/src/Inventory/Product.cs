namespace Inventory;

public sealed class Product
{
    public const int SkuMaxLength = 20;

    private Product()
    {
    }

    public Product(string sku, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Sku = sku;
        Name = name;
    }

    public int Id { get; private set; }
    public string Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int OnHand { get; private set; }

    public void Receive(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        OnHand += quantity;
    }

    public void Reserve(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        if (quantity > OnHand)
        {
            throw new InsufficientStockException(Sku, quantity, OnHand);
        }

        OnHand -= quantity;
    }
}

public sealed class InsufficientStockException(string sku, int requested, int onHand)
    : Exception($"Cannot reserve {requested} of {sku}: only {onHand} on hand.")
{
    public string Sku { get; } = sku;
}
