namespace Accounts.Tests;

// TODO-01: Read src/Accounts/ApiTokenIssuer.cs, including the nested Entry record. A token is valid
// for 15 minutes. Test both sides of the limit, with the fake clock at a fixed instant.
[TestClass]
public sealed class ApiTokenIssuerTests
{
    [TestMethod]
    public void ATokenIsValidRightAfterItIsIssued()
    {
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ATokenIsStillValidOneTickBeforeFifteenMinutes()
    {
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ATokenExpiresExactlyFifteenMinutesAfterItIsIssued()
    {
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void ATokenThatWasNeverIssuedIsUnknown()
    {
        Assert.Fail("TODO-01: write this test.");
    }
}
