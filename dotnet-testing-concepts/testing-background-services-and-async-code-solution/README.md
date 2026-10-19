# Testing BackgroundService, Channels and Async Code (Solution)

Official solution for the `testing-background-services-and-async-code` lab. 8 tests, about 0.2 s, and
one real wait in the whole suite (a 200 ms quiet period, explained below).

## What changed from the starter

- **`RecordingLedger`** writes every batch it receives into a `Channel<IReadOnlyList<StockMovement>>`. That
  channel is the signal: `NextCallAsync` awaits the next batch with `WaitAsync(5 s, token)`, so a broken
  worker fails the test after 5 s instead of hanging it, and a working one costs nothing.
- **`SignalingTimeProvider`** overrides `CreateTimer` and completes a `TaskCompletionSource`. Since .NET 10
  `ExecuteAsync` runs on a background thread, so after `StartAsync` the timer may not exist yet. A
  `FakeTimeProvider.Advance` before that moment moves time for nobody: the timer is created afterwards and
  counts from the new "now". Every test waits for `TimerCreated` before it touches the clock.
- **`NothingIsAppliedBeforeTheFirstTick`** is the one place with a real wait. "Nothing happened" has no
  signal, so after the worker exists and the clock is 1 ms before the tick, the ledger gets a 200 ms quiet
  period. Then a second movement is written and the first call must hold both. Without the quiet period
  the test passed against a worker that flushes on arrival, because the test wrote both movements before
  that worker woke up.
- **`AFailedBatchIsLoggedAndAppliedAgainOnTheNextTick`** checks the three things a failure must leave:
  the same batch again on the next tick, `ExecuteTask` still running, and one `Error` record in
  `FakeLogger.Collector` with the exception and the formatted message. Asserting the log after the retry
  arrives removes the race: the worker logs before it waits for the next tick.
- **`CompletingTheChannelDrainsItAndFinishesTheWorker`** awaits `ExecuteTask` with a deadline. A bare
  `await` would hang the run if the worker never finishes.
- **`StockBatchingHostTests`** use `Host.CreateApplicationBuilder`, register the fake clock before
  `AddStockBatching` (which uses `TryAddSingleton(TimeProvider.System)`), and collect logs with
  `AddFakeLogging()`. When a `BackgroundService` throws, the host logs an `Error` ("BackgroundService
  failed") and a `Critical` entry, then stops (`BackgroundServiceExceptionBehavior.StopHost`, the default
  since .NET 6). The test asserts the opposite: one `Error` from the worker, no `Critical`, and
  `ApplicationStopping` not cancelled.
- **`StopAsyncDoesNotRethrowWhatKilledTheWorker`** (optional) shows why: `ExecuteTask` faults, and
  `StopAsync` still completes normally.

## Bonus notes

Mutation check, run against this solution. Each change to `StockBatchWorker.cs` was tried on its own, and
the file was restored afterwards:

| Change to the production code | Tests that fail |
| --- | --- |
| `FlushAsync` ignores `MaxBatchSize` | `ABatchNeverExceedsMaxBatchSize` |
| The `catch` filter becomes `when (false)`: a ledger failure escapes `ExecuteAsync` | `AFailedBatchIsLoggedAndAppliedAgainOnTheNextTick`, `AFailingLedgerDoesNotStopTheHost` |
| The batch is cleared before the ledger call, so a failed batch is lost | `AFailedBatchIsLoggedAndAppliedAgainOnTheNextTick`, `AFailingLedgerDoesNotStopTheHost` |
| No `return` after the channel completes | `CompletingTheChannelDrainsItAndFinishesTheWorker` |
| No final `FlushAsync(CancellationToken.None)` on stop | `StoppingAppliesWhatIsAlreadyQueued`, `TheHostRunsTheWorkerAndDrainsTheQueueOnShutdown` |
| `reader.WaitToReadAsync` instead of the timer: flush on arrival | `NothingIsAppliedBeforeTheFirstTick` and both failure tests |

Two naive tests were also run against those mutations: "StartAsync, write, assert no call" stayed green
with flush on arrival, and "the ledger failed once, StopAsync completed" stayed green with the escaping
exception. Both were green for the wrong reason.

What no test here covers: a ledger that hangs forever (add a timeout around `ApplyAsync` and test it with
the fake clock), and a crash during the final drain, which loses the queued movements by design.
