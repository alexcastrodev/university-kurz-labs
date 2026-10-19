namespace Accounts.Tests;

// TODO-00: Read src/Accounts/Trials.cs, then write the four tests below.
// - Create a FakeTimeProvider (already referenced) at a fixed instant, for example 2026-01-05 09:00 UTC.
//   Not DateTimeOffset.UtcNow: a fake that sits at the real "now" hides code that still reads the real clock.
// - Move time with Advance. Thread.Sleep and Task.Delay in a test do not move a FakeTimeProvider at all.
// - Write the 14 days in the test. TrialService.Length would make the test agree with any value.
[TestClass]
public sealed class TrialTests
{
    [TestMethod]
    public void ATrialIsActiveRightAfterItStarts()
    {
        Assert.Fail("TODO-00: write this test.");
    }

    [TestMethod]
    public void ATrialIsStillActiveOneTickBeforeFourteenDays()
    {
        // TimeSpan.FromTicks(1) is the smallest step a TimeSpan can take.
        Assert.Fail("TODO-00: write this test.");
    }

    [TestMethod]
    public void ATrialExpiresExactlyFourteenDaysAfterItStarts()
    {
        // Exactly 14 days, not 15: a test that jumps well past the limit passes even when the limit is wrong.
        Assert.Fail("TODO-00: write this test.");
    }

    [TestMethod]
    public void TheRemainingTimeCountsDown()
    {
        Assert.Fail("TODO-00: write this test.");
    }
}
