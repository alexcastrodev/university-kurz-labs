# Operators, Null Handling and Null-Conditional Assignment

## Goal

Use `?.`, `?[]`, `??`, `??=` and the C# 14 null-conditional assignment correctly, and avoid the precedence and lifted-comparison traps that come with them.

## Prerequisites

- Nullable reference types and `?.` for reading
- Events and indexers
- .NET 10 SDK (C# 14)

## Task

`Operations` is stubbed. Each method is a one-line null-aware operation. The tests count how many times the right-hand side runs: with `customer?.Name = newName()` the call happens only when `customer` is not `null`.

## Instructions

Complete the following TODOs:

- TODO-00: `Rename`: `?.` as the target of `=`, right side skipped for null.
- TODO-01: `Birthday`: compound assignment `+=` and its nullable value.
- TODO-02: `EnsureNickname`: `?.` combined with `??=`.
- TODO-03: `SetCustomerName`: a chain that stops at the first null.
- TODO-04: `Subscribe`: `?.` with `+=` on an event.
- TODO-05: `Put`: `?[]` as an assignment target.
- TODO-06: `IsShort`: the lifted comparison trap of `?.Length < 2`.
- TODO-07: `Total`: `??` has lower precedence than `+`.
- TODO-08: `BothValid`: the eager `&` when both operands must run.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-09 (optional): Add `BirthdayOf(Func<Customer?> getCustomer)` that adds ten years with `getCustomer()?.Age += 10`, and prove the receiver is evaluated only once.
- TODO-10 (optional): Add `TryRename(Customer?, string)` that uses the assignment as a value (`string? written = customer?.Name = name`) and returns whether anything was written. What is the type of that expression?
