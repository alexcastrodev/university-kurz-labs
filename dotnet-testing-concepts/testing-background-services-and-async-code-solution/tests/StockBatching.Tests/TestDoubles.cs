using System.Threading.Channels;
using Microsoft.Extensions.Time.Testing;

namespace StockBatching.Tests;

// A ledger that records every call and lets the test wait for the next one. No Task.Delay anywhere:
// the test awaits a signal, and the deadline only costs time when the test is already failing.
internal sealed class RecordingLedger : IStockLedger
{
    private static readonly TimeSpan Deadline = TimeSpan.FromSeconds(5);

    private readonly Channel<IReadOnlyList<StockMovement>> _calls = Channel.CreateUnbounded<IReadOnlyList<StockMovement>>();
    private int _failuresLeft;

    // The next `count` calls record the batch and then throw, like a database that is down.
    public void FailNext(int count) => Volatile.Write(ref _failuresLeft, count);

    public Task ApplyAsync(IReadOnlyList<StockMovement> batch, CancellationToken cancellationToken)
    {
        _calls.Writer.TryWrite(batch);
        return Interlocked.Decrement(ref _failuresLeft) >= 0
            ? Task.FromException(new InvalidOperationException("The ledger is down."))
            : Task.CompletedTask;
    }

    public async Task<IReadOnlyList<StockMovement>> NextCallAsync(CancellationToken cancellationToken) =>
        await _calls.Reader.ReadAsync(cancellationToken).AsTask().WaitAsync(Deadline, cancellationToken);

    // Proving that something does NOT happen needs a quiet period: there is no signal for "nothing".
    // It is the only real wait in the suite, so keep it short and keep it to the "not yet" rule.
    public async Task<bool> IsCalledWithinAsync(TimeSpan quietPeriod, CancellationToken cancellationToken)
    {
        var call = _calls.Reader.WaitToReadAsync(cancellationToken).AsTask();
        return await Task.WhenAny(call, Task.Delay(quietPeriod, cancellationToken)) == call;
    }
}

// A FakeTimeProvider that tells the test when the worker has created its timer. Advancing the clock
// before that moment moves time for nobody: the timer is created later and counts from the new "now".
internal sealed class SignalingTimeProvider : FakeTimeProvider
{
    private readonly TaskCompletionSource _timerCreated = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task TimerCreated => _timerCreated.Task;

    public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
    {
        var timer = base.CreateTimer(callback, state, dueTime, period);
        _timerCreated.TrySetResult();
        return timer;
    }
}
