# Records and with Expressions

## Goal

Copy and change immutable records with `with`, understand that the copy is shallow, and keep invariants valid even when `with` bypasses constructors.

## Prerequisites

- C# classes and properties
- `init` accessors
- .NET 10 SDK

## Task

`Operations` and `Money` are stubbed. `Customer` holds an `Address` and a list of tags, so you will meet both the nested `with` and the shallow copy trap.

## Instructions

Complete the following TODOs:

- TODO-00: `MoveTo`: nested `with` expressions without touching the original.
- TODO-01: `AddTag`: a new list instead of mutating the shared one.
- TODO-02: `Money`: validate in the `init` accessor so `with` cannot create a negative amount.
- TODO-03: `Add`: build the result from the first operand with `with`.
- TODO-04: `SameContent`: compare what record equality compares by reference.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Turn `Money` into a `readonly record struct`. What changes for `with`, equality and the validation?
- TODO-06 (optional): Derive `record DiscountedMoney : Money` and compare a `Money` and a `DiscountedMoney` with the same values. Why are they not equal?
