namespace Shop;

public interface IPaymentGateway
{
    Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken);
}

public sealed class PaymentDeclinedException(decimal amount)
    : InvalidOperationException($"The payment of {amount} was declined.")
{
    public decimal Amount { get; } = amount;
}

public sealed record Receipt(decimal Total, IReadOnlyDictionary<string, int> Lines);

// Adding to the cart reserves stock right away. A checkout that does not complete
// (declined, failed or cancelled) gives every reserved unit back and empties the cart.
public sealed class Cart(Inventory inventory)
{
    private readonly Dictionary<string, int> _lines = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, int> Lines => _lines;

    public decimal Total => _lines.Sum(line => inventory.PriceOf(line.Key) * line.Value);

    public void Add(string sku, int quantity)
    {
        inventory.Reserve(sku, quantity);
        _lines[sku] = _lines.GetValueOrDefault(sku) + quantity;
    }

    public void Remove(string sku)
    {
        if (_lines.Remove(sku, out var quantity))
        {
            inventory.Release(sku, quantity);
        }
    }

    public async Task<Receipt> CheckoutAsync(IPaymentGateway payments, CancellationToken cancellationToken)
    {
        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("The cart is empty.");
        }

        var total = Total;
        try
        {
            if (!await payments.ChargeAsync(total, cancellationToken))
            {
                throw new PaymentDeclinedException(total);
            }
        }
        catch
        {
            ReleaseAll();
            throw;
        }

        var receipt = new Receipt(total, new Dictionary<string, int>(_lines));
        _lines.Clear();
        return receipt;
    }

    private void ReleaseAll()
    {
        foreach (var (sku, quantity) in _lines)
        {
            inventory.Release(sku, quantity);
        }

        _lines.Clear();
    }
}
