# Primary Constructors for Classes and Structs

## Goal

Learn what a primary constructor parameter really is in a class or struct (a captured, mutable, hidden field, not a property) and how to avoid its traps.

## Prerequisites

- C# classes, structs and constructors
- Dependency injection basics
- a .NET 10 SDK (C# 14)

## Task

`Models` has five types written with primary constructors. The compiler already warns (CS9124, CS9107) about two of them: those warnings point at state that can drift apart.

## Instructions

Complete the following TODOs:

- TODO-00: `Account`: remove the double storage of `owner`.
- TODO-01: `OrderService`: readonly field plus argument validation in the initializer.
- TODO-02: `Counter` struct: parameterless constructor chained to the primary one.
- TODO-03: `Savings`: stop capturing parameters that the base class already stores.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-04 (optional): `Coordinates`: turn it into a positional record.
- TODO-05 (optional): `Service`: a readonly field with the same name shadows the parameter.
