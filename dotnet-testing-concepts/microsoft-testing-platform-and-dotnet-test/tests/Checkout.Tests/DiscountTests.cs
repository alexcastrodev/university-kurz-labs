namespace Checkout.Tests;

// TODO-02: The tier discount, the coupons and the cap. Each test asserts the Discount (or the Total) it is about.
// TODO-04: Tag this class with [TestCategory("Discounts")].
[TestClass]
public sealed class DiscountTests
{
    private static Quote Price(decimal amount, Tier tier = Tier.Regular, string? coupon = null) =>
        PriceCalculator.Calculate([new("ITEM", 1, amount)], tier, coupon);

    [TestMethod]
    public void TheTierDiscountDependsOnTheTier()
    {
        // One [DataRow] per tier on a subtotal of 100. Attribute arguments cannot be decimal:
        // pass a double and convert it with (decimal)expected.
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void Welcome10NeedsASubtotalOfAtLeast50()
    {
        // Test both sides of the limit: exactly 50 and just below it.
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void ACouponIgnoresCaseAndSurroundingSpaces()
    {
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void AnUnknownCouponIsRejected()
    {
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void TheTierDiscountAndTheCouponAddUp()
    {
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void TheDiscountNeverExceedsTheSubtotal()
    {
        // FLAT20 on an order of 15: what is the discount, and what does the customer pay in total?
        Assert.Fail("TODO-02: write this test.");
    }
}
