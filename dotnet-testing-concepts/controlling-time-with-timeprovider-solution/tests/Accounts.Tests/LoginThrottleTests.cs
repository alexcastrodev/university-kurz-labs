namespace Accounts.Tests;

[TestClass]
public sealed class LoginThrottleTests
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 5, 9, 0, 0, TimeSpan.Zero));

    [TestMethod]
    public void ThreeAttemptsAreAllowedAndTheFourthIsRefused()
    {
        using var throttle = new LoginThrottle(3, TimeSpan.FromMinutes(1), _clock);

        Assert.IsTrue(throttle.TryAttempt());
        Assert.IsTrue(throttle.TryAttempt());
        Assert.IsTrue(throttle.TryAttempt());
        Assert.IsFalse(throttle.TryAttempt());
    }

    [TestMethod]
    public void TheWindowResetsExactlyWhenTheMinuteEnds()
    {
        using var throttle = new LoginThrottle(3, TimeSpan.FromMinutes(1), _clock);
        for (var i = 0; i < 3; i++)
        {
            throttle.TryAttempt();
        }

        // Both sides of the boundary. Advancing two minutes would pass even if the reset came late.
        _clock.Advance(TimeSpan.FromMinutes(1) - TimeSpan.FromTicks(1));
        Assert.IsFalse(throttle.TryAttempt());

        _clock.Advance(TimeSpan.FromTicks(1)); // the timer callback runs inside Advance
        Assert.IsTrue(throttle.TryAttempt());
    }
}
