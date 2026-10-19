namespace Accounts.Tests;

[TestClass]
public sealed class DeliveryRetryTests(TestContext testContext)
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 5, 9, 0, 0, TimeSpan.Zero));

    // The pattern: start the task, move the clock, then await. Awaiting first never returns, because
    // nobody is left to call Advance. [Timeout] turns that hang into a failure.
    [TestMethod]
    [Timeout(2000)]
    public async Task TheAttemptsFollowTheBackoffOfOneTwoAndFourSeconds()
    {
        var attempts = 0;
        var run = new DeliveryRetry(_clock).RunAsync(_ => Task.FromResult(++attempts == 4), testContext.CancellationToken);
        Assert.AreEqual(1, attempts);

        _clock.Advance(TimeSpan.FromSeconds(1) - TimeSpan.FromTicks(1));
        Assert.AreEqual(1, attempts);

        _clock.Advance(TimeSpan.FromTicks(1));
        Assert.AreEqual(2, attempts);

        _clock.Advance(TimeSpan.FromSeconds(2));
        Assert.AreEqual(3, attempts);

        _clock.Advance(TimeSpan.FromSeconds(4));
        Assert.IsTrue(await run);
        Assert.AreEqual(4, attempts);
    }

    [TestMethod]
    [Timeout(2000)]
    public async Task AnAttemptThatHangsIsAbandonedAtTheTenSecondDeadline()
    {
        var run = new DeliveryRetry(_clock).RunAsync(
            async ct =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, ct); // a server that never answers
                return true;
            },
            testContext.CancellationToken);

        _clock.Advance(TimeSpan.FromSeconds(10) - TimeSpan.FromTicks(1));
        Assert.IsFalse(run.IsCompleted);

        _clock.Advance(TimeSpan.FromTicks(1));
        Assert.IsFalse(await run);
    }
}
