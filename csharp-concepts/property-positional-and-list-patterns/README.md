# Property, Positional and List Patterns

## Goal

Match the shape of objects, tuples and sequences with property, positional and list patterns, and know what each one costs and when it cannot be used.

## Prerequisites

- Type, constant and relational patterns
- Records and `switch` expressions
- .NET 10 SDK

## Task

`Patterns` is stubbed. Each method is a small real-world case: a null-safe customer filter, a coordinate classifier, a door state machine, a command parser and a sequence describer.

## Instructions

Complete the following TODOs:

- TODO-00: `IsLisbonRegular`: extended property pattern, null-safe through `Address.City`.
- TODO-01: `Quadrant`: positional pattern with relational patterns and discards.
- TODO-02: `Next`: tuple `switch` as a state machine, with a throwing discard arm.
- TODO-03: `Describe`: list patterns with a captured slice.
- TODO-04: `Inner`: list pattern with a slice on a `string`.
- TODO-05: `Parse`: command parser, where arm order and `when` matter.
- TODO-06: `IsOrigin`: positional pattern on a boxed tuple and a type pattern for a record.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Add `Sum(ReadOnlySpan<int>)` using `[var head, .. var tail]` recursion. Why does it not allocate for a span but would for an `int[]`?
- TODO-08 (optional): Add a class `Rgb` with your own `Deconstruct` and an `IsGray(Rgb)` method that matches it with a positional pattern.
