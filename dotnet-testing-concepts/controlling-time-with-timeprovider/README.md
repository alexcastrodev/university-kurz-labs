# Controlling Time in Tests with TimeProvider and FakeTimeProvider

## Goal

Five small account rules depend on time: a 14-day trial, a 15-minute API token, a login throttle that
resets every minute with a timer, quiet hours in the customer's time zone, and a delivery retry with
backoff and a deadline. They are already implemented, and every one of them reads time through an
injected `TimeProvider`. Your job is to write the MSTest tests with `FakeTimeProvider`, so they run in
milliseconds, never sleep, and test each limit exactly where it is.

## Prerequisites

- Test Doubles in .NET: Fakes, Stubs and NSubstitute (the concept and its lab)
- `async`/`await` and `CancellationToken`
- .NET 10 SDK

## Task

Read `src/Accounts` first. Then open the test project. Every test method is already declared with a
hint, and fails with `TODO-NN: write this test` until you write it.

Three traps are waiting:

- `Thread.Sleep` and `Task.Delay` in a test do not move a `FakeTimeProvider`, and awaiting the code
  under test before you call `Advance` never returns;
- a fake clock started at `DateTimeOffset.UtcNow` agrees with code that forgot to use it;
- an `Advance` that jumps well past a limit passes even when the limit is wrong; one that stops short
  of it proves nothing either. Test one tick before and exactly at the limit.

## Instructions

Complete the following TODOs in `tests/Accounts.Tests`:

- TODO-00: `TrialTests`: active after the start, still active one tick before 14 days, expired at
  exactly 14 days, remaining time.
- TODO-01: `ApiTokenIssuerTests`: valid, valid one tick before 15 minutes, expired at 15 minutes, unknown.
- TODO-02: `LoginThrottleTests`: three attempts per minute, and the reset exactly when the minute ends.
- TODO-03: `QuietHoursTests`: a `[DataRow]` table of local times in a custom time zone.
- TODO-04: `DeliveryRetryTests`: the backoff schedule and the 10-second deadline (start, advance, await).

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): The forgotten clock. In `ApiTokenIssuer`, make the nested `Entry.IsExpiredAt`
  read `DateTimeOffset.UtcNow` instead of its `now` parameter. Which of your tests fail? Now start your
  fake at `DateTimeOffset.UtcNow` and run them again. Restore both afterwards.
- TODO-06 (optional): Mutation check. Change the production code on purpose, one change at a time:
  `<` to `<=` in `TrialService.StatusOf`, `>=` to `>` in `Entry.IsExpiredAt`, a throttle timer that
  fires one second late, `Task.Delay(wait, linked.Token)` without the provider, and `GetUtcNow` in
  `QuietHours`. Each change should turn at least one test red.
