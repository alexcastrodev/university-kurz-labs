namespace Accounts.Tests;

[TestClass]
public sealed class ApiTokenIssuerTests
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 5, 9, 0, 0, TimeSpan.Zero));

    private static readonly TimeSpan FifteenMinutes = TimeSpan.FromMinutes(15);

    [TestMethod]
    public void ATokenIsValidRightAfterItIsIssued()
    {
        var issuer = new ApiTokenIssuer(_clock);

        var token = issuer.Issue(Guid.NewGuid());

        Assert.AreEqual(TokenCheck.Valid, issuer.Check(token));
    }

    [TestMethod]
    public void ATokenIsStillValidOneTickBeforeFifteenMinutes()
    {
        var issuer = new ApiTokenIssuer(_clock);
        var token = issuer.Issue(Guid.NewGuid());

        _clock.Advance(FifteenMinutes - TimeSpan.FromTicks(1));

        Assert.AreEqual(TokenCheck.Valid, issuer.Check(token));
    }

    [TestMethod]
    public void ATokenExpiresExactlyFifteenMinutesAfterItIsIssued()
    {
        var issuer = new ApiTokenIssuer(_clock);
        var token = issuer.Issue(Guid.NewGuid());

        _clock.Advance(FifteenMinutes);

        Assert.AreEqual(TokenCheck.Expired, issuer.Check(token));
    }

    [TestMethod]
    public void ATokenThatWasNeverIssuedIsUnknown()
    {
        var issuer = new ApiTokenIssuer(_clock);

        Assert.AreEqual(TokenCheck.Unknown, issuer.Check("not-a-token"));
    }
}
