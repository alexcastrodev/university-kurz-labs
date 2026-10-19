namespace Accounts.Tests;

[TestClass]
public sealed class TrialTests
{
    // A fixed instant, far from the real "now". Code that reads the real clock anywhere shows up at once.
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 5, 9, 0, 0, TimeSpan.Zero));

    private static readonly Guid Customer = Guid.NewGuid();

    // The 14 days are written here on purpose. Using TrialService.Length would make the test agree with
    // whatever the production constant says, including a wrong value.
    private static readonly TimeSpan FourteenDays = TimeSpan.FromDays(14);

    [TestMethod]
    public void ATrialIsActiveRightAfterItStarts()
    {
        var trials = new TrialService(_clock);

        var trial = trials.Start(Customer);

        Assert.AreEqual(TrialStatus.Active, trials.StatusOf(trial));
        Assert.AreEqual(_clock.GetUtcNow(), trial.StartedAt);
    }

    [TestMethod]
    public void ATrialIsStillActiveOneTickBeforeFourteenDays()
    {
        var trials = new TrialService(_clock);
        var trial = trials.Start(Customer);

        _clock.Advance(FourteenDays - TimeSpan.FromTicks(1));

        Assert.AreEqual(TrialStatus.Active, trials.StatusOf(trial));
    }

    [TestMethod]
    public void ATrialExpiresExactlyFourteenDaysAfterItStarts()
    {
        var trials = new TrialService(_clock);
        var trial = trials.Start(Customer);

        _clock.Advance(FourteenDays);

        Assert.AreEqual(TrialStatus.Expired, trials.StatusOf(trial));
        Assert.AreEqual(TimeSpan.Zero, trials.Remaining(trial));
    }

    [TestMethod]
    public void TheRemainingTimeCountsDown()
    {
        var trials = new TrialService(_clock);
        var trial = trials.Start(Customer);

        _clock.Advance(TimeSpan.FromDays(3));

        Assert.AreEqual(TimeSpan.FromDays(11), trials.Remaining(trial));
    }
}
