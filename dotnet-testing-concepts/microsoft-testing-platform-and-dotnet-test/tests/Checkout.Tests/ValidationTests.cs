namespace Checkout.Tests;

// TODO-01: The input checks at the top of PriceCalculator.Calculate. Read src/Checkout/PriceCalculator.cs first.
// TODO-04: Tag this class with [TestCategory("Validation")].
[TestClass]
public sealed class ValidationTests
{
    [TestMethod]
    public void AnOrderWithoutLinesIsRejected()
    {
        // Assert.ThrowsExactly<T> wants exactly T. Which type does Calculate throw for an empty list?
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ALineWithoutAPositiveQuantityIsRejected()
    {
        // Make this one test that runs for 0 and for -1: give it an int parameter and add [DataRow] rows.
        // Put the bad line second, after a valid one, so the loop has to reach it.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void TheSubtotalAddsQuantityTimesPriceOfEveryLine()
    {
        // Two lines with quantities above 1, so a subtotal that ignores the quantity gives a different number.
        Assert.Fail("TODO-01: write this test.");
    }
}
