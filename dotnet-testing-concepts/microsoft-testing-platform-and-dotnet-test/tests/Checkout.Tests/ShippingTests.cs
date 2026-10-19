namespace Checkout.Tests;

// TODO-03: Shipping. Careful: your discount tests already execute both shipping branches, so the coverage
// report counts them as covered even though nothing asserts the shipping yet. These tests are the ones that do.
// TODO-04: Tag this class with [TestCategory("Shipping")].
[TestClass]
public sealed class ShippingTests
{
    private static Quote Price(decimal amount, Tier tier = Tier.Regular, string? coupon = null) =>
        PriceCalculator.Calculate([new("ITEM", 1, amount)], tier, coupon);

    [TestMethod]
    public void AnOrderBelowTheThresholdPaysTheFee()
    {
        // Check the Shipping and the Total.
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void AnOrderOfExactlyTheThresholdShipsFree()
    {
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void TheThresholdIsCheckedAfterTheDiscount()
    {
        // An order of 110 with FLAT20 is 90 after the discount.
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void GoldShipsFreeEvenBelowTheThreshold()
    {
        Assert.Fail("TODO-03: write this test.");
    }
}
