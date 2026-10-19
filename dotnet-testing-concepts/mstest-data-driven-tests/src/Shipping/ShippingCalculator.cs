namespace Shipping;

public enum Zone
{
    Domestic,
    Neighbor,
    International,
}

// A parcel weighing exactly UpToGrams still belongs to the band.
public sealed record WeightBand(int UpToGrams, decimal BasePrice);

public static class ShippingCalculator
{
    public const int MaxWeightGrams = 30_000;
    public const decimal FreeShippingThreshold = 100m;
    public const string FreeShippingCode = "FREESHIP";

    public static IReadOnlyList<WeightBand> Bands { get; } =
    [
        new(1_000, 5.00m),
        new(5_000, 9.50m),
        new(20_000, 18.00m),
        new(MaxWeightGrams, 32.00m),
    ];

    // Domestic orders ship free from FreeShippingThreshold on, or with the FREESHIP code
    // (case-insensitive, surrounding spaces ignored). Other zones always pay.
    public static decimal Quote(int weightGrams, Zone zone, decimal orderTotal = 0m, string? promoCode = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(weightGrams);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(weightGrams, MaxWeightGrams);
        ArgumentOutOfRangeException.ThrowIfNegative(orderTotal);
        var multiplier = Multiplier(zone);

        if (zone == Zone.Domestic && (orderTotal >= FreeShippingThreshold || IsFreeShippingCode(promoCode)))
        {
            return 0m;
        }

        var band = Bands.First(band => weightGrams <= band.UpToGrams);
        return band.BasePrice * multiplier;
    }

    // Every parcel of an order is priced on its own band.
    public static decimal QuoteParcels(Zone zone, params int[] parcelWeightsGrams)
    {
        if (parcelWeightsGrams.Length == 0)
        {
            throw new ArgumentException("An order needs at least one parcel.", nameof(parcelWeightsGrams));
        }

        return parcelWeightsGrams.Sum(grams => Quote(grams, zone));
    }

    private static decimal Multiplier(Zone zone) => zone switch
    {
        Zone.Domestic => 1.0m,
        Zone.Neighbor => 1.5m,
        Zone.International => 2.5m,
        _ => throw new ArgumentOutOfRangeException(nameof(zone), zone, "Unknown zone."),
    };

    private static bool IsFreeShippingCode(string? promoCode) =>
        string.Equals(promoCode?.Trim(), FreeShippingCode, StringComparison.OrdinalIgnoreCase);
}
