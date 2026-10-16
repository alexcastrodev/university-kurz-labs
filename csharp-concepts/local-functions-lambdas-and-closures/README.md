# Local Functions, Lambdas and Closures

## Goal

Capture variables in lambdas on purpose, avoid the shared loop variable trap, and use local functions for eager validation in iterators.

## Prerequisites

- C# delegates (`Func`, `Action`)
- Lambdas
- Iterators (`yield return`)
- a .NET 10 SDK

## Task

`Functions` is stubbed. Each method returns a delegate or a lazy sequence, so what you capture (and when) decides whether the tests pass.

## Instructions

Complete the following TODOs:

- TODO-00: `MakeCounter`: a captured local that survives between calls.
- TODO-01: `MakeReaders`: one captured value per loop iteration.
- TODO-02: `Squares`: validate eagerly, iterate in a `static` local function.
- TODO-03: `IntParser`: method group conversion instead of a wrapper lambda.
- TODO-04: `Memoize`: a captured dictionary as the cache.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): a lambda with a default parameter value (C# 12).
- TODO-06 (optional): a lambda with an `out` parameter and no declared types (C# 14).
- TODO-07 (optional): a `static` lambda that cannot capture anything.
