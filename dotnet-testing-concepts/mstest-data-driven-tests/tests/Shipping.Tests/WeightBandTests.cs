namespace Shipping.Tests;

// TODO-00: A colleague wrote the test below as a loop, and it passes. Break the band lookup on purpose: in
// src/Shipping/ShippingCalculator.cs change `weightGrams <= band.UpToGrams` to `<`. Still green. Then swap
// the prices of the 5 kg and 20 kg bands: red, but only the first wrong value is reported.
// Undo both changes and replace the loop with one row per case, on both sides of every band edge
// (1 g, 1 000 g, 1 001 g, ... 30 000 g). A [DataRow] cannot hold a decimal (try [DataRow(1_000, 5.00)] and
// read the build warning and the failure), so use a [DynamicData] source of (int Grams, decimal Expected) tuples.
[TestClass]
public sealed class WeightBandTests
{
    [TestMethod]
    public void ADomesticParcelIsPricedByItsWeightBand()
    {
        foreach (var (grams, expected) in new[] { (500, 5.00m), (3_000, 9.50m), (12_000, 18.00m), (25_000, 32.00m) })
        {
            Assert.AreEqual(expected, ShippingCalculator.Quote(grams, Zone.Domestic));
        }

        Assert.Fail("TODO-00: this loop passes for the wrong reason. Turn it into data rows on the band edges.");
    }
}
