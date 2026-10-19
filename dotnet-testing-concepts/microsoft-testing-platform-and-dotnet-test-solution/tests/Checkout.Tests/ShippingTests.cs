namespace Checkout.Tests;

[TestClass]
[TestCategory("Shipping")]
public sealed class ShippingTests
{
    private static Quote Price(decimal amount, Tier tier = Tier.Regular, string? coupon = null) =>
        PriceCalculator.Calculate([new("ITEM", 1, amount)], tier, coupon);

    [TestMethod]
    public void AnOrderBelowTheThresholdPaysTheFee()
    {
        var quote = Price(99.99m);

        Assert.AreEqual(PriceCalculator.ShippingFee, quote.Shipping);
        Assert.AreEqual(107.49m, quote.Total);
    }

    [TestMethod]
    public void AnOrderOfExactlyTheThresholdShipsFree() =>
        Assert.AreEqual(0m, Price(100m).Shipping);

    [TestMethod]
    public void TheThresholdIsCheckedAfterTheDiscount() =>
        Assert.AreEqual(PriceCalculator.ShippingFee, Price(110m, coupon: "FLAT20").Shipping);

    [TestMethod]
    public void GoldShipsFreeEvenBelowTheThreshold() =>
        Assert.AreEqual(0m, Price(20m, Tier.Gold).Shipping);
}
