using Microsoft.VisualStudio.TestTools.UnitTesting.Combinatorial;

namespace Shipping.Tests;

[TestClass]
public sealed class ZoneMatrixTests
{
    // Every zone (all values of the enum are inferred) times four weights: 3 x 4 = 12 cases.
    // Each new parameter multiplies the count, so keep the values to the edges that matter.
    [TestMethod]
    [CombinatorialData]
    public void EveryValidParcelPaysSomethingBelowTheThreshold(
        Zone zone,
        [CombinatorialValues(1, 1_000, 1_001, 30_000)] int grams) =>
        Assert.IsPositive(ShippingCalculator.Quote(grams, zone));
}
