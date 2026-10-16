# Fields, Constants and Initialization Order

## Goal

Observe the exact order in which C# initializes an object, and choose the right kind of field (`const`, `static readonly`, `required`) for each job.

## Prerequisites

- C# classes, constructors and inheritance
- `const`, `readonly` and `static`
- a .NET 10 SDK (C# 14)

## Task

`Models` holds small types that each expose one initialization rule. Some tests inspect the compiled shape of a type with reflection, because the rule is a property of the IL, not of runtime output.

## Instructions

Complete the following TODOs:

- TODO-00: `Base`/`Derived`: call a virtual method from the base constructor and see which field is already set.
- TODO-01: `Limits`: `const` for the value that never changes, `static readonly` for the tunable one.
- TODO-02: `Precise`: add a static constructor so the type loses `beforefieldinit`.
- TODO-03: `Person`: `required` members and `[SetsRequiredMembers]` on the full constructor.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-04 (optional): `Counter<T>`: keep the count in a static field of the generic type itself.
- TODO-05 (optional): `Settings`: chain constructors with `this(...)` and validate in the full one.
