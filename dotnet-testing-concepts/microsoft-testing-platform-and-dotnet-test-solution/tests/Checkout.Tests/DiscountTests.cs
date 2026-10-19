namespace Checkout.Tests;

[TestClass]
[TestCategory("Discounts")]
public sealed class DiscountTests
{
    private static Quote Price(decimal amount, Tier tier = Tier.Regular, string? coupon = null) =>
        PriceCalculator.Calculate([new("ITEM", 1, amount)], tier, coupon);

    // Attribute arguments cannot be decimal, so the rows carry doubles and the test converts them.
    [TestMethod]
    [DataRow(Tier.Regular, 0.0)]
    [DataRow(Tier.Silver, 5.0)]
    [DataRow(Tier.Gold, 10.0)]
    public void TheTierDiscountDependsOnTheTier(Tier tier, double expected) =>
        Assert.AreEqual((decimal)expected, Price(100m, tier).Discount);

    [TestMethod]
    [DataRow(50.0, 10.0)]
    [DataRow(49.99, 0.0)]
    public void Welcome10NeedsASubtotalOfAtLeast50(double subtotal, double expected) =>
        Assert.AreEqual((decimal)expected, Price((decimal)subtotal, coupon: "WELCOME10").Discount);

    [TestMethod]
    public void ACouponIgnoresCaseAndSurroundingSpaces() =>
        Assert.AreEqual(10m, Price(60m, coupon: "  welcome10 ").Discount);

    [TestMethod]
    public void AnUnknownCouponIsRejected() =>
        Assert.ThrowsExactly<ArgumentException>(() => Price(60m, coupon: "FREE100"));

    [TestMethod]
    public void TheTierDiscountAndTheCouponAddUp() =>
        Assert.AreEqual(25m, Price(100m, Tier.Silver, "FLAT20").Discount);

    [TestMethod]
    public void TheDiscountNeverExceedsTheSubtotal()
    {
        var quote = Price(15m, coupon: "FLAT20");

        Assert.AreEqual(15m, quote.Discount);
        Assert.AreEqual(PriceCalculator.ShippingFee, quote.Total);
    }
}
