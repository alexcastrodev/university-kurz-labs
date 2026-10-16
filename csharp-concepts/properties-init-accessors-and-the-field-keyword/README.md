# Properties, init Accessors and the field Keyword

## Goal

Put validation and lazy defaults inside auto-implemented properties with the `field` keyword (C# 14), and use `init` accessors for construction-time rules.

## Prerequisites

- C# properties and auto-properties
- Events
- Exceptions
- a .NET 10 SDK (C# 14)

## Task

`Models` holds four classes with plain auto-properties. Each one needs logic in an accessor, and `field` lets you add it without declaring a backing field.

## Instructions

Complete the following TODOs:

- TODO-00: `Temperature.Celsius`: validate in the setter.
- TODO-01: `Person.Name`: trim and validate in an `init` accessor.
- TODO-02: `Document.Title`: lazy default with `field ??=`.
- TODO-03: `Counter.Value`: raise `Changed` only when the value changes.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-04 (optional): `Price.Amount`: normalize in an `init` accessor.
- TODO-05 (optional): `Session.Token`: attach `[field: NonSerialized]` to the compiler-generated backing field.
