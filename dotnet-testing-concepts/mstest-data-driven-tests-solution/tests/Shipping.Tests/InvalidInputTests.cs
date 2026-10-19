namespace Shipping.Tests;

[TestClass]
public sealed class InvalidInputTests
{
    [TestMethod]
    [DataRow(0, DisplayName = "zero grams")]
    [DataRow(-1, DisplayName = "a negative weight")]
    [DataRow(30_001, DisplayName = "one gram over the limit")]
    [DataRow(int.MaxValue)]
    public void AWeightOutsideTheLimitsIsRejected(int grams)
    {
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ShippingCalculator.Quote(grams, Zone.Domestic));

        Assert.AreEqual("weightGrams", error.ParamName);
    }

    // Every check throws ArgumentOutOfRangeException, so the type alone cannot tell which rule fired.
    // With an invalid weight in the call the weight check would fire first, and the test would pass even
    // with the order total check deleted. A valid weight and ParamName pin the rule down.
    [TestMethod]
    public void ANegativeOrderTotalIsRejected()
    {
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => ShippingCalculator.Quote(500, Zone.Domestic, orderTotal: -0.01m));

        Assert.AreEqual("orderTotal", error.ParamName);
    }

    [TestMethod]
    [DataRow((Zone)99)]
    public void AnUnknownZoneIsRejected(Zone zone)
    {
        var error = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ShippingCalculator.Quote(500, zone));

        Assert.AreEqual("zone", error.ParamName);
    }
}
