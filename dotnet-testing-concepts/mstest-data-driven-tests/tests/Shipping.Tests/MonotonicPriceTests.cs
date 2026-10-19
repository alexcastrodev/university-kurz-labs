namespace Shipping.Tests;

// TODO-05: Use [BandEdges] here, with parameters (int lastGramOfBand, Zone zone). Assert a property, not a
// price: one gram past a band edge costs more (Assert.IsGreaterThan(lowerBound, value)).
[TestClass]
public sealed class MonotonicPriceTests
{
    [TestMethod]
    public void OneGramPastABandEdgeCostsMore()
    {
        Assert.Fail("TODO-05: write this test.");
    }
}
