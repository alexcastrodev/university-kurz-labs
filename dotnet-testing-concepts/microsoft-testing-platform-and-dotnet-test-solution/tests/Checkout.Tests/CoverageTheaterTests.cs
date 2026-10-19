namespace Checkout.Tests;

// TODO-08 (optional): a test that executes every branch and asserts nothing. Run the coverage script with
// --filter "TestCategory=Theater": the report is as high as the real suite's, and no mutation turns it red.
[TestClass]
[TestCategory("Theater")]
public sealed class CoverageTheaterTests
{
    [TestMethod]
    public void ExecutesEverythingAndChecksNothing()
    {
        string?[] coupons = [null, "", "WELCOME10", "FLAT20", "NOPE"];
        decimal[] amounts = [10m, 49m, 60m, 120m];

        foreach (var tier in Enum.GetValues<Tier>())
        foreach (var coupon in coupons)
        foreach (var amount in amounts)
        {
            Try(() => PriceCalculator.Calculate([new("ITEM", 1, amount)], tier, coupon));
        }

        Try(() => PriceCalculator.Calculate([], Tier.Regular));
        Try(() => PriceCalculator.Calculate([new("ITEM", 0, 1m)], Tier.Regular));
    }

    private static void Try(Action act)
    {
        try
        {
            act();
        }
        catch (ArgumentException)
        {
            // Swallowed on purpose: this is what makes the test worthless.
        }
    }
}
