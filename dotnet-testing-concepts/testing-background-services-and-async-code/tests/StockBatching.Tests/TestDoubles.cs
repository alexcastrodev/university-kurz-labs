using System.Threading.Channels;
using Microsoft.Extensions.Time.Testing;

namespace StockBatching.Tests;

// TODO-00: Write a ledger the tests can wait on. No Task.Delay: the test awaits a signal.
//   - ApplyAsync records the batch it received (a Channel<IReadOnlyList<StockMovement>> is a handy
//     "signal with a payload"), then throws InvalidOperationException if FailNext asked for it.
//   - NextCallAsync waits for the next recorded call, with a deadline of a few seconds
//     (WaitAsync(TimeSpan, CancellationToken)) so a broken worker fails the test instead of hanging it.
//   - IsCalledWithinAsync answers "was the ledger called during this short window?". It is the only
//     honest real wait in the suite: there is no signal for "nothing happened".
internal sealed class RecordingLedger : IStockLedger
{
    public void FailNext(int count) =>
        throw new NotImplementedException("TODO-00: make the next `count` calls throw.");

    public Task ApplyAsync(IReadOnlyList<StockMovement> batch, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-00: record the batch, then fail if asked to.");

    public Task<IReadOnlyList<StockMovement>> NextCallAsync(CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-00: wait for the next call, with a deadline.");

    public Task<bool> IsCalledWithinAsync(TimeSpan quietPeriod, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-00: true if a call arrives within the quiet period.");
}

// TODO-01: Make this FakeTimeProvider complete TimerCreated when the worker creates its timer
// (override CreateTimer, call the base, then signal a TaskCompletionSource). Advancing the clock before
// the timer exists moves time for nobody: the timer is created later and counts from the new "now".
internal sealed class SignalingTimeProvider : FakeTimeProvider
{
    public Task TimerCreated => throw new NotImplementedException("TODO-01: signal when a timer is created.");
}
