namespace Shipping.Tests;

[TestClass]
public sealed class WeightBandTests
{
    // Both sides of every band edge, plus the lightest and heaviest valid parcels. The starter's loop only
    // checked the middle of each band (500 g, 3 kg, 12 kg, 25 kg), so changing <= to < in the band lookup
    // left it green, and it stopped at the first wrong value. Each row here is reported on its own.
    // A decimal cannot be an attribute argument, so these rows live in a DynamicData source of tuples.
    public static IEnumerable<(int Grams, decimal Expected)> BandEdges =>
    [
        (1, 5.00m),
        (1_000, 5.00m),
        (1_001, 9.50m),
        (5_000, 9.50m),
        (5_001, 18.00m),
        (20_000, 18.00m),
        (20_001, 32.00m),
        (30_000, 32.00m),
    ];

    [TestMethod]
    [DynamicData(nameof(BandEdges))]
    public void ADomesticParcelIsPricedByItsWeightBand(int grams, decimal expected) =>
        Assert.AreEqual(expected, ShippingCalculator.Quote(grams, Zone.Domestic));
}
