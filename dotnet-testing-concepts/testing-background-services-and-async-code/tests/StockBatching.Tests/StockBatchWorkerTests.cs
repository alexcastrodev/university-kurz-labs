using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace StockBatching.Tests;

// The worker without a host: construct it, StartAsync, drive the fake clock, StopAsync.
[TestClass]
public sealed class StockBatchWorkerTests(TestContext testContext) : IDisposable
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    private readonly Channel<StockMovement> _channel = Channel.CreateUnbounded<StockMovement>();
    private readonly RecordingLedger _ledger = new();
    private readonly SignalingTimeProvider _time = new();
    private readonly FakeLogger<StockBatchWorker> _logger = new();
    private StockBatchWorker? _worker = null; // TODO-02 assigns it

    // Pass this token to everything you await. It is cancelled when the test times out or the run aborts.
    private CancellationToken Token => testContext.CancellationToken;

    // TODO-02: Create the worker (Interval, maxBatchSize), call StartAsync and return it.
    // Since .NET 10 ExecuteAsync runs on a background thread: after StartAsync the worker may not have
    // created its timer yet. Wait for _time.TimerCreated (with a deadline) before the test advances time.
    private Task<StockBatchWorker> StartWorkerAsync(int maxBatchSize = 100) =>
        throw new NotImplementedException("TODO-02: start the worker and wait until its timer exists.");

    private void Write(params StockMovement[] movements)
    {
        foreach (var movement in movements)
        {
            Assert.IsTrue(_channel.Writer.TryWrite(movement));
        }
    }

    public void Dispose() => _worker?.Dispose();

    [TestMethod]
    public async Task NothingIsAppliedBeforeTheFirstTick()
    {
        // TODO-02: Write one movement, advance to 1 ms before the tick, check that the ledger is not called
        // during a short quiet period, write a second movement, advance the last millisecond, and assert
        // that the FIRST call holds both movements. Asserting "no call" right after StartAsync is the trap:
        // it passes even for a worker that applies everything immediately, because it has not run yet.
        await Task.CompletedTask;
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public async Task ABatchNeverExceedsMaxBatchSize()
    {
        // TODO-03: maxBatchSize 2, five movements, one tick: the ledger sees 2, 2 and 1.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task AFailedBatchIsLoggedAndAppliedAgainOnTheNextTick()
    {
        // TODO-03: The ledger fails once. Prove three things: the same batch arrives again on the next tick,
        // the worker is still running (ExecuteTask), and exactly one Error was logged with the exception
        // (_logger.Collector.GetSnapshot()). Trap: "the ledger was called, then StopAsync completed" is
        // green even when the exception killed the worker. StopAsync does not rethrow it.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task CompletingTheChannelDrainsItAndFinishesTheWorker()
    {
        // TODO-04: Write, Complete the writer, tick once. All movements are applied and ExecuteTask
        // completes successfully. Await it with a deadline, never with a bare await.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task StoppingAppliesWhatIsAlreadyQueued()
    {
        // TODO-04: Write three movements, StopAsync without any tick: the ledger still receives them.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task StopAsyncDoesNotRethrowWhatKilledTheWorker()
    {
        // TODO-06 (optional): Write a tiny BackgroundService whose ExecuteAsync throws after an await.
        // Start it, prove that ExecuteTask faults with Assert.ThrowsExactlyAsync, then show that StopAsync
        // completes normally. This is why a hosted-service test must look at ExecuteTask or the logs.
        await Task.CompletedTask;
        Assert.Inconclusive("TODO-06 (optional)");
    }
}
