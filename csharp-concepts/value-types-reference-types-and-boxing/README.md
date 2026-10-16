# Value Types, Reference Types and Boxing

## Goal

See copy semantics for structs, fix the classic mutable-struct bugs, find where boxing
allocates, and use `stackalloc` to keep a buffer off the heap.

## Prerequisites

- C# classes and structs
- `ref` parameters
- .NET 10 SDK

## Task

`Counter` is a mutable struct, `Temperature` is a mutable struct that should be immutable, and
`Holder` hides a counter in a `readonly` field. `Semantics` and `Boxing` have methods that need
a body. The allocation tests use `GC.GetAllocatedBytesForCurrentThread` with wide margins and a
warm-up call, so they compare orders of magnitude instead of exact bytes.

## Instructions

Complete the following TODOs:

- TODO-00: `IncrementCopy` works on a copy.
- TODO-01: `IncrementRef` changes the caller's struct.
- TODO-02: Turn `Temperature` into a `readonly struct` with `WithCelsius`.
- TODO-03: Fix `Holder` so the increments are not lost.
- TODO-04: `UnboxAll` unboxes every element.
- TODO-05: `SumOnStack` with `stackalloc`.
- TODO-06: `BoxNullable` and what `int?` boxes to.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Add `Semantics.Length(in Vector3D vector)` and pass a `readonly struct`
  by readonly reference. Add a `Vector3D` readonly struct with `X`, `Y`, `Z`.
- TODO-08 (optional): Add `Boxing.AllocatedByFormat(int)` and `Boxing.AllocatedByInterpolation(int)`
  to compare `string.Format("{0}", i)` with `$"{i}"` using allocated bytes.
- TODO-09 (optional): Add `Semantics.IncrementFirst(List<Counter>)`. `list[0].Increment()` does
  not compile, so use `CollectionsMarshal.AsSpan`.
