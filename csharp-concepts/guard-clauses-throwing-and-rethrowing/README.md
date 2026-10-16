# Guard Clauses, Throwing and Rethrowing

## Goal

Write guard clauses with the BCL `ThrowIf` helpers and `CallerArgumentExpression`, keep throw code out of hot methods, and rethrow or wrap exceptions without losing the stack trace.

## Prerequisites

- Exceptions: `try`, `catch` and `finally`
- Attributes and optional parameters
- `async`/`await` basics
- .NET 10 SDK

## Task

`Guards` is stubbed. The first half is about usage errors (a caller bug, thrown as early as possible with a precise message), the second half about what to do with a failure once it is caught.

## Instructions

Complete the following TODOs:

- TODO-00: `Place`: one-line guards with `ArgumentNullException.ThrowIfNull`, `ThrowIfNullOrWhiteSpace`, `ThrowIfNegativeOrZero` and `ThrowIfGreaterThan`.
- TODO-01: `NotDefault`: your own guard, with the argument text coming from `CallerArgumentExpression`.
- TODO-02: `FirstArg`: a `throw` expression inside a conditional.
- TODO-03: `Get`: test the bounds and delegate the throw to a helper.
- TODO-04: `ThrowIndex`: the `[DoesNotReturn]` throw helper that keeps `Get` small.
- TODO-05: `SaveAsync`: validate eagerly, because an `async` method stores its exceptions in the `Task`.
- TODO-06: `LogAndRethrow`: `throw;` keeps the stack trace, `throw ex;` resets it.
- TODO-07: `LoadSetting`: wrap with meaning and keep the original as `InnerException`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Add `RethrowLater(Action, List<ExceptionDispatchInfo>)` that captures the exception in a `catch` and lets the caller rethrow it later with the original stack trace.
- TODO-09 (optional): Add a `Resource : IDisposable` class whose `Read()` is guarded with `ObjectDisposedException.ThrowIf`.
