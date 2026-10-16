# Span, Memory and Slicing Without Allocating

## Goal

Process text and arrays through `Span<T>` slices without copying, know why `Memory<T>` exists,
and use `stackalloc`, `params ReadOnlySpan<T>` and `[InlineArray]`.

## Prerequisites

- C# arrays and strings
- Value types versus reference types
- .NET 10 SDK

## Task

`Slicing` has six methods that need a body, and `Buffers.Squares` fills an inline array. The
allocation tests call each method many times, after a warm-up, and compare allocated bytes
with wide margins. They separate "a few bytes" from "a new array or string per call", not
exact byte counts.

## Instructions

Complete the following TODOs:

- TODO-00: `CountWords` without `Split`.
- TODO-01: `ParseInts` into a caller-owned span.
- TODO-02: `Middle` as a writable view.
- TODO-03: `Sum` with `params ReadOnlySpan<int>`.
- TODO-04: `ToHex` with a `stackalloc` buffer.
- TODO-05: `SumAsync` with `Memory<int>`.
- TODO-06: `Buffers.Squares` with an `[InlineArray]`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Add `StartsWithZero(this ReadOnlySpan<int> span)` and call it directly on
  an `int[]`. The C# 14 first-class span conversions make this bind.
- TODO-08 (optional): Add `AsBytes(ReadOnlySpan<int>)` that views the ints as bytes with
  `MemoryMarshal.AsBytes`.
