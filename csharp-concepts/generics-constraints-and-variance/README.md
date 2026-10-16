# Generics: Constraints and Variance

## Goal

Write generic methods with the right constraints, see that generics are reified, avoid
boxing on interface calls, and mark interfaces as covariant or contravariant.

## Prerequisites

- C# interfaces and inheritance
- Value types versus reference types
- .NET 10 SDK

## Task

`Generics` has five generic methods with the constraints removed, `TypeCounter<T>` keeps
state per closed type, and `Variance.cs` declares `IProducer<T>` and `IConsumer<T>` without
variance. Several tests use reflection to check the constraint or the variance flag, because
a missing constraint is a compile-time property and cannot be asserted any other way.
Two tests measure allocated bytes to prove that a constrained call does not box.

## Instructions

Complete the following TODOs:

- TODO-00: `Max<T>` with an `IComparable<T>` constraint.
- TODO-01: `Create<T>` with `class, new()`.
- TODO-02: `ByteCount<T>` with `unmanaged`.
- TODO-03: `Describe<T>` using `typeof(T)` and `default(T)`.
- TODO-04: `TypeCounter<T>.Increment`.
- TODO-05: `CompareViaConstraint<T>` without boxing.
- TODO-06: Make `IProducer<T>` covariant.
- TODO-07: Make `IConsumer<T>` contravariant.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Add `Generics.NameOf<T>(T value)` that returns `typeof(T).Name` and
  accepts `ReadOnlySpan<char>` by using the C# 13 `allows ref struct` anti-constraint.
- TODO-09 (optional): Try `object[] boxes = new string[1]; boxes[0] = 42;` and read the
  exception. Why does `IReadOnlyList<object>` not have this problem?
