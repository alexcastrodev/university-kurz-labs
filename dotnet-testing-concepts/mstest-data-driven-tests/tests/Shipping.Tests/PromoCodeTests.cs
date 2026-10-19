namespace Shipping.Tests;

// TODO-02: The FREESHIP code makes a domestic parcel free. Case and surrounding spaces do not matter.
[TestClass]
public sealed class PromoCodeTests
{
    [TestMethod]
    public void TheFreeShippingCodeMakesADomesticParcelFree()
    {
        // Rows: the code as written, lower case, and with spaces around it.
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void AnyOtherCodePaysTheBandPrice()
    {
        // Rows: no code at all ([DataRow(null)] with a string? parameter), an empty string, and near misses.
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void TheCodeDoesNotWorkAbroad()
    {
        Assert.Fail("TODO-02: write this test.");
    }
}
