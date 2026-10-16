# Type, Constant and Relational Patterns

## Goal

Replace if/else chains with patterns: type, constant, relational, logical (`and`, `or`, `not`), tuple patterns and switch expressions, and see what exhaustiveness means.

## Prerequisites

- `switch` statements
- `is` operator
- .NET 10 SDK

## Task

`Patterns` has six methods stubbed. Every one has a short list of cases in the TODO and tests with the edges (boundaries, undefined enum values).

## Instructions

Complete the following TODOs:

- TODO-00: `Describe`: type, constant and relational patterns in one switch expression.
- TODO-01: `Grade`: relational patterns with `or` for the invalid range.
- TODO-02: `IsWeekday`: `is not (... or ...)`.
- TODO-03: `IsAsciiLetter`: ranges combined with `and` / `or`.
- TODO-04: `TicketPrice`: tuple pattern, arms evaluated top to bottom.
- TODO-05: `Next`: an exhaustive-looking switch with no discard arm and the runtime consequence.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Add an arm to `Describe` that the compiler flags as unreachable (for example `int` before `int and < 0`). Read the error.
- TODO-07 (optional): Rewrite `TicketPrice` with a `when` clause instead of a tuple pattern and compare readability.
