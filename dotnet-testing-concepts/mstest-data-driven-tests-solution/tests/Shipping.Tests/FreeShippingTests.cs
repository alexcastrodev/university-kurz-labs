namespace Shipping.Tests;

[TestClass]
public sealed class FreeShippingTests
{
    [TestMethod]
    [DynamicData(nameof(ShippingCases.Thresholds), typeof(ShippingCases))]
    public void FreeShippingStartsAtTheThresholdForDomesticOrdersOnly(decimal orderTotal, Zone zone, decimal expected) =>
        Assert.AreEqual(expected, ShippingCalculator.Quote(500, zone, orderTotal));
}
