namespace Shipping.Tests;

// TODO-03: QuoteParcels prices every parcel on its own band and adds the prices up.
[TestClass]
public sealed class ParcelTests
{
    [TestMethod]
    public void EveryParcelIsPricedOnItsOwnBand()
    {
        // Rows with one, two and three parcels. Signature hint: (int expectedCents, params int[] parcelGrams):
        // an int in cents fits in a [DataRow], and params takes the rest of the row.
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void AnOrderWithoutParcelsIsRejected()
    {
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void OneInvalidParcelRejectsTheWholeOrder()
    {
        // An array is a valid attribute argument: [DataRow(new[] { 500, 0 })] with an int[] parameter.
        Assert.Fail("TODO-03: write this test.");
    }
}
