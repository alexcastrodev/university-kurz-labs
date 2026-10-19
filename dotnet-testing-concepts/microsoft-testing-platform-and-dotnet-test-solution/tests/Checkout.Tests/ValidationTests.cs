namespace Checkout.Tests;

[TestClass]
[TestCategory("Validation")]
public sealed class ValidationTests
{
    [TestMethod]
    public void AnOrderWithoutLinesIsRejected() =>
        Assert.ThrowsExactly<ArgumentException>(() => PriceCalculator.Calculate([], Tier.Regular));

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ALineWithoutAPositiveQuantityIsRejected(int quantity) =>
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => PriceCalculator.Calculate([new("BOOK", 1, 10m), new("PEN", quantity, 2m)], Tier.Regular));

    [TestMethod]
    public void TheSubtotalAddsQuantityTimesPriceOfEveryLine()
    {
        var quote = PriceCalculator.Calculate([new("BOOK", 2, 12.50m), new("PEN", 3, 1.20m)], Tier.Regular);

        Assert.AreEqual(28.60m, quote.Subtotal);
    }
}
