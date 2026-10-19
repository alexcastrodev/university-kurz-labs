namespace Shipping.Tests;

[TestClass]
public sealed class MonotonicPriceTests
{
    [TestMethod]
    [BandEdges]
    public void OneGramPastABandEdgeCostsMore(int lastGramOfBand, Zone zone)
    {
        var atTheEdge = ShippingCalculator.Quote(lastGramOfBand, zone);
        var pastTheEdge = ShippingCalculator.Quote(lastGramOfBand + 1, zone);

        Assert.IsGreaterThan(atTheEdge, pastTheEdge);
    }
}
