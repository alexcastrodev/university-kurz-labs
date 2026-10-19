namespace Accounts.Tests;

// TODO-02: Read src/Accounts/LoginThrottle.cs. It resets its counter with a timer created by the
// TimeProvider. Dispose the throttle at the end of each test (using var).
[TestClass]
public sealed class LoginThrottleTests
{
    [TestMethod]
    public void ThreeAttemptsAreAllowedAndTheFourthIsRefused()
    {
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public void TheWindowResetsExactlyWhenTheMinuteEnds()
    {
        // Use up the three attempts. Then check one tick before the minute ends (still refused) and at
        // the minute (allowed again). Advancing two minutes would also pass if the reset came late.
        Assert.Fail("TODO-02: write this test.");
    }
}
