namespace Shipping.Tests;

[TestClass]
public sealed class ParcelTests
{
    // The expected total is in cents: a [DataRow] cannot carry a decimal, but it can carry an int.
    // The parcels go last, into a params array, so each row can have a different number of them.
    [TestMethod]
    [DataRow(500, 500)]
    [DataRow(1_450, 500, 1_500)]
    [DataRow(5_500, 500, 5_001, 20_001)]
    public void EveryParcelIsPricedOnItsOwnBand(int expectedCents, params int[] parcelGrams) =>
        Assert.AreEqual(expectedCents / 100m, ShippingCalculator.QuoteParcels(Zone.Domestic, parcelGrams));

    [TestMethod]
    public void AnOrderWithoutParcelsIsRejected() =>
        Assert.ThrowsExactly<ArgumentException>(() => ShippingCalculator.QuoteParcels(Zone.Domestic));

    [TestMethod]
    [DataRow(new[] { 500, 0 })]
    [DataRow(new[] { 30_001, 500 })]
    public void OneInvalidParcelRejectsTheWholeOrder(int[] parcelGrams) =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ShippingCalculator.QuoteParcels(Zone.Domestic, parcelGrams));
}
