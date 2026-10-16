# Exceptions: try, catch, Filters and finally

## Goal

Practice ordering catch clauses, exception filters, `finally`, `ExceptionDispatchInfo` and `AggregateException`, and see for yourself that filters run before inner `finally` blocks.

## Prerequisites

- Basic `try`/`catch`
- Exception class hierarchy (`IOException`, `FileNotFoundException`)
- `Task` basics
- .NET 10 SDK

## Task

`Handlers` and `OrderRejectedException` are stubbed. The tests describe the expected behavior, including the order of the log entries in the two-pass model.

## Instructions

Complete the following TODOs:

- TODO-00: `Classify`: ordered catch clauses plus a `when` filter on `RemoteException`.
- TODO-01: `RunWithCleanup`: `finally` runs on success and on failure, and the exception still propagates.
- TODO-02: `ObserveTwoPasses`: nested `try/finally` under a `catch ... when`; observe filter, finally, catch.
- TODO-03: `TryCapture`: `ExceptionDispatchInfo.Capture` to rethrow later keeping the original stack trace.
- TODO-04: `CollectMessages`: unwrap an `AggregateException` from `Task.WhenAll` with `Flatten()`.
- TODO-05: `OrderRejectedException`: custom exception that keeps the inner exception and an `OrderId`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Replace `throw e;` for `throw;` in a catch block you write and compare the stack traces. Which frames disappear?
- TODO-07 (optional): Measure with `Stopwatch` how long 100000 throw/catch cycles take versus 100000 `int.TryParse` failures.
