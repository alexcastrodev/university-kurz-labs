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
    private StockBatchWorker? _worker;

    private CancellationToken Token => testContext.CancellationToken;

    private async Task<StockBatchWorker> StartWorkerAsync(int maxBatchSize = 100)
    {
        _worker = new StockBatchWorker(
            _channel.Reader, _ledger, _time, new StockBatchOptions { Interval = Interval, MaxBatchSize = maxBatchSize }, _logger);
        await _worker.StartAsync(Token);
        // Since .NET 10 ExecuteAsync runs on a background thread, so after StartAsync the worker may not
        // have created its timer yet. Advancing the clock before this point would move time for nobody.
        await _time.TimerCreated.WaitAsync(TimeSpan.FromSeconds(5), Token);
        return _worker;
    }

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
        await StartWorkerAsync();
        Write(new StockMovement("KB-01", 5));

        _time.Advance(Interval - TimeSpan.FromMilliseconds(1));
        // Asserting "no call yet" right after StartAsync proves nothing: the worker may not have run at all.
        // Here the worker has created its timer, so a worker that applies early has had its chance.
        Assert.IsFalse(await _ledger.IsCalledWithinAsync(TimeSpan.FromMilliseconds(200), Token));
        Write(new StockMovement("MS-02", -1));
        _time.Advance(TimeSpan.FromMilliseconds(1));

        // If the worker had applied anything before the tick, the first call would hold only KB-01.
        var first = await _ledger.NextCallAsync(Token);
        CollectionAssert.AreEqual(new StockMovement[] { new("KB-01", 5), new("MS-02", -1) }, first.ToArray());
    }

    [TestMethod]
    public async Task ABatchNeverExceedsMaxBatchSize()
    {
        await StartWorkerAsync(maxBatchSize: 2);
        Write(new StockMovement("A", 1), new StockMovement("B", 1), new StockMovement("C", 1), new StockMovement("D", 1), new StockMovement("E", 1));

        _time.Advance(Interval);

        Assert.HasCount(2, await _ledger.NextCallAsync(Token));
        Assert.HasCount(2, await _ledger.NextCallAsync(Token));
        Assert.HasCount(1, await _ledger.NextCallAsync(Token));
    }

    [TestMethod]
    public async Task AFailedBatchIsLoggedAndAppliedAgainOnTheNextTick()
    {
        var worker = await StartWorkerAsync();
        _ledger.FailNext(1);
        Write(new StockMovement("KB-01", 5), new StockMovement("MS-02", -1));

        _time.Advance(Interval);
        var failed = await _ledger.NextCallAsync(Token);
        _time.Advance(Interval);
        var retried = await _ledger.NextCallAsync(Token);

        CollectionAssert.AreEqual(failed.ToArray(), retried.ToArray());
        // The worker must still be alive. StopAsync would not tell you: it does not rethrow.
        Assert.IsFalse(worker.ExecuteTask!.IsCompleted);
        var error = Assert.ContainsSingle(_logger.Collector.GetSnapshot().Where(r => r.Level == LogLevel.Error));
        Assert.IsInstanceOfType<InvalidOperationException>(error.Exception);
        Assert.AreEqual("Failed to apply 2 stock movements; retrying on the next tick", error.Message);
    }

    [TestMethod]
    public async Task CompletingTheChannelDrainsItAndFinishesTheWorker()
    {
        var worker = await StartWorkerAsync();
        Write(new StockMovement("KB-01", 5), new StockMovement("MS-02", -1));
        _channel.Writer.Complete();

        _time.Advance(Interval);

        Assert.HasCount(2, await _ledger.NextCallAsync(Token));
        await worker.ExecuteTask!.WaitAsync(TimeSpan.FromSeconds(5), Token);
        Assert.IsTrue(worker.ExecuteTask.IsCompletedSuccessfully);
    }

    [TestMethod]
    public async Task StoppingAppliesWhatIsAlreadyQueued()
    {
        var worker = await StartWorkerAsync();
        Write(new StockMovement("KB-01", 5), new StockMovement("MS-02", -1), new StockMovement("HD-03", 2));

        await worker.StopAsync(Token); // no tick happened

        Assert.HasCount(3, await _ledger.NextCallAsync(Token));
        Assert.IsTrue(worker.ExecuteTask!.IsCompletedSuccessfully);
    }

    // TODO-06 (optional) in the starter.
    [TestMethod]
    public async Task StopAsyncDoesNotRethrowWhatKilledTheWorker()
    {
        var worker = new ExplodingWorker();

        await worker.StartAsync(Token);
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => worker.ExecuteTask!);
        await worker.StopAsync(Token); // completes normally: the failure is only in ExecuteTask

        Assert.IsTrue(worker.ExecuteTask!.IsFaulted);
    }

    private sealed class ExplodingWorker : Microsoft.Extensions.Hosting.BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            throw new InvalidOperationException("boom");
        }
    }
}
