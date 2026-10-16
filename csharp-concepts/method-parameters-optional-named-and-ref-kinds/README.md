# Method Parameters: Optional, Named and ref Kinds

## Goal

Use optional and named arguments, the parameter passing modes (`ref`, `out`, `in`, `ref readonly`) and `params` collections, and predict overload resolution.

## Prerequisites

- C# methods and structs
- Tuples and deconstruction (basic)
- a .NET 10 SDK

## Task

`Methods` is stubbed. Tests call the methods with `ref`/`out`, named arguments and different numeric types, so the signatures are already fixed.

## Instructions

Complete the following TODOs:

- TODO-00: `Swap`: exchange the caller's variables with `ref`.
- TODO-01: `TryDivide`: assign every `out` parameter on every path.
- TODO-02: `Heaviest`: return a `ref readonly` to the element, not a copy.
- TODO-03: `Sum`: `params ReadOnlySpan<int>` (C# 13).
- TODO-04: `BuildUrl`: optional parameters and named arguments.
- TODO-05, TODO-06, TODO-07: the three `Pick` overloads. Predict which one `Pick(5)` binds to before running the tests.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): `FindSlot`: return a `ref int` so the caller can write into the array element.
- TODO-09 (optional): `CountValid`: use the discard `_` for an `out` value you do not need.
