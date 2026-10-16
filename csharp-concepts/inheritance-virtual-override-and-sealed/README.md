# Inheritance: virtual, override, new and sealed

## Goal

Predict and control which method runs: `virtual`/`override` dispatch by runtime type, `new` hides by static type, `sealed` and `abstract` shape the hierarchy.

## Prerequisites

- C# classes and inheritance
- Type patterns and `switch` expressions
- a .NET 10 SDK (C# 14)

## Task

`Models` contains a small hierarchy of animals, reports and shapes, plus a `CountingBag` that inherits from `Bag` and miscounts. Fix each type until the tests agree.

## Instructions

Complete the following TODOs:

- TODO-00: `Animal`/`Dog`: make `Speak()` a virtual method overridden by `Dog`.
- TODO-01: `Report`: abstract base with a protected constructor and a sealed subclass.
- TODO-02: `Clone()`: virtual in `Animal`, covariant override in `Dog`.
- TODO-03: `ShapeText.Describe`: a `switch` expression with type and property patterns.
- TODO-04: `CountingBag`: replace inheritance by composition to escape the fragile base class.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): `Circle.Area()`: make it a `sealed override`.
