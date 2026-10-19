namespace Shipping.Tests;

// TODO-01: Every invalid input throws ArgumentOutOfRangeException. Read the order of the checks in Quote.
[TestClass]
public sealed class InvalidInputTests
{
    [TestMethod]
    public void AWeightOutsideTheLimitsIsRejected()
    {
        // One test, several [DataRow] rows: 0, -1, 30 001 and int.MaxValue. Give the first three a DisplayName
        // and compare how the rows are named in the output.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ANegativeOrderTotalIsRejected()
    {
        // Careful: if the call also has an invalid weight, which check fires? Assert.ThrowsExactly returns the
        // exception: its ParamName tells which rule rejected the call.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void AnUnknownZoneIsRejected()
    {
        // An enum value can be an attribute argument, and so can a cast: [DataRow((Zone)99)].
        Assert.Fail("TODO-01: write this test.");
    }
}
