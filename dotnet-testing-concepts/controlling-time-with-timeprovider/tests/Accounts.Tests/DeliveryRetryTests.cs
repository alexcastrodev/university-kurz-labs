namespace Accounts.Tests;

// TODO-04: Read src/Accounts/DeliveryRetry.cs. It waits 1 s, 2 s and 4 s between attempts with
// Task.Delay(wait, time, token) and gives up after 10 s with a CancellationTokenSource(Deadline, time).
// The pattern is: start the task (do not await it yet), Advance the clock, then await at the end.
// If you await first, nobody is left to call Advance and the test hangs; [Timeout] makes that a failure.
[TestClass]
public sealed class DeliveryRetryTests(TestContext testContext)
{
    [TestMethod]
    [Timeout(2000)]
    public async Task TheAttemptsFollowTheBackoffOfOneTwoAndFourSeconds()
    {
        // Count the attempts in the delegate and let the fourth one succeed. Check the counter after
        // each Advance, including one tick before the first second.
        await Task.Delay(0, testContext.CancellationToken);
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    [Timeout(2000)]
    public async Task AnAttemptThatHangsIsAbandonedAtTheTenSecondDeadline()
    {
        // An attempt that never answers: await Task.Delay(Timeout.InfiniteTimeSpan, ct) inside the delegate.
        // One tick before 10 s the run is not finished; at 10 s it returns false.
        await Task.Delay(0, testContext.CancellationToken);
        Assert.Fail("TODO-04: write this test.");
    }
}
