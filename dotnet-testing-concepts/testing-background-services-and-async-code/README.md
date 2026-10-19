# Testing BackgroundService, Channels and Async Code

## Goal

`StockBatchWorker` is already fully implemented: a `BackgroundService` that reads stock movements from a
`Channel<StockMovement>` and applies them to a ledger in batches, once per interval, on a `PeriodicTimer`.
Your job is to write the MSTest tests that prove its five rules, without a single `Task.Delay` used as a
"wait for the worker", in a suite that runs in a fraction of a second.

## Prerequisites

- Hosted Services and BackgroundService, and In-Memory Queues with Channels (dotnet-concepts)
- Controlling Time with TimeProvider (this track): `FakeTimeProvider` and `Advance`
- Basic MSTest: `[TestClass]`, `[TestMethod]`, `TestContext`
- .NET 10 SDK

## Task

Read `src/StockBatching/StockBatchWorker.cs` first. The comment at the top lists the five rules. Then open
the test project. Every test is declared with a hint and fails with `TODO-NN: write this test` until you
write it. The two helpers in `TestDoubles.cs` are shells for now.

Three traps are waiting, and each one makes a naive test pass for the wrong reason:

- **The test that passes before the worker runs.** Since .NET 10 `ExecuteAsync` runs on a background
  thread, so right after `StartAsync` the worker may not have done anything yet. "No call yet" asserted at
  that moment is green even for a worker that applies every movement immediately.
- **The exception swallowed by the worker.** `BackgroundService.StopAsync` does not rethrow what killed
  `ExecuteAsync`. A test that only checks "the ledger was called and StopAsync completed" stays green when
  the first failure kills the worker for good.
- **`Task.Delay` in the test.** It does not move a `FakeTimeProvider` at all, and with the real clock it
  turns a 5 second interval into a slow, flaky test. Advance the fake clock and await a signal instead.

## Instructions

Complete the following TODOs in `tests/StockBatching.Tests`:

- TODO-00: `RecordingLedger`: record each call, fail on demand, let the test await the next call with a
  deadline, and answer "was it called during this short window?".
- TODO-01: `SignalingTimeProvider`: a `FakeTimeProvider` that signals when the worker creates its timer.
- TODO-02: `StartWorkerAsync`, and the test that nothing is applied before the first tick.
- TODO-03: batches never exceed `MaxBatchSize`; a failed batch is logged (`FakeLogger`) and applied again
  on the next tick while the worker keeps running.
- TODO-04: completing the channel drains it and finishes the worker; stopping applies what is queued.
- TODO-05: the same worker inside a real generic host: drain on shutdown, and a failing ledger that does
  not stop the host.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): prove that `StopAsync` does not rethrow the exception that faulted `ExecuteTask`,
  with a tiny `BackgroundService` written inside the test class.
- TODO-07 (optional): mutation check. Change `StockBatchWorker` on purpose, one change at a time, and see
  which of your tests turn red: ignore `MaxBatchSize`; change the `catch` filter to `when (false)`; clear
  the batch before calling the ledger; remove the `return` after the channel completes; remove the final
  `FlushAsync(CancellationToken.None)`; replace `timer.WaitForNextTickAsync` with
  `reader.WaitToReadAsync`. A change that leaves everything green is a missing test.
