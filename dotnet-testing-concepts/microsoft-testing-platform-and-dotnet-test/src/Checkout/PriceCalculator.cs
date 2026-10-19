namespace Checkout;

public enum Tier
{
    Regular,
    Silver,
    Gold,
}

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

public sealed record Quote(decimal Subtotal, decimal Discount, decimal Shipping, decimal Total);

// Prices an order: subtotal, a tier discount, at most one coupon, and shipping.
public static class PriceCalculator
{
    public const decimal FreeShippingThreshold = 100m;
    public const decimal ShippingFee = 7.50m;

    public static Quote Calculate(IReadOnlyList<OrderLine> lines, Tier tier, string? coupon = null)
    {
        if (lines.Count == 0)
        {
            throw new ArgumentException("An order needs at least one line.", nameof(lines));
        }

        foreach (var line in lines)
        {
            if (line.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lines), $"Line {line.Sku} has quantity {line.Quantity}.");
            }
        }

        var subtotal = lines.Sum(l => l.Quantity * l.UnitPrice);
        var discount = Math.Min(subtotal, TierDiscount(tier, subtotal) + CouponDiscount(coupon, subtotal));
        var discounted = subtotal - discount;

        // Two conditions on one line: a single test can cover the line without covering both branches.
        var shipping = discounted >= FreeShippingThreshold || tier == Tier.Gold ? 0m : ShippingFee;

        return new Quote(subtotal, discount, shipping, discounted + shipping);
    }

    private static decimal TierDiscount(Tier tier, decimal subtotal) => tier switch
    {
        Tier.Silver => Math.Round(subtotal * 0.05m, 2),
        Tier.Gold => Math.Round(subtotal * 0.10m, 2),
        _ => 0m,
    };

    private static decimal CouponDiscount(string? coupon, decimal subtotal) =>
        coupon?.Trim().ToUpperInvariant() switch
        {
            null or "" => 0m,
            "WELCOME10" => subtotal >= 50m ? 10m : 0m,
            "FLAT20" => 20m,
            _ => throw new ArgumentException($"Unknown coupon '{coupon}'.", nameof(coupon)),
        };
}
