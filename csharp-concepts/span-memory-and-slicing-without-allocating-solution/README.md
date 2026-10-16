# Span, Memory and Slicing Without Allocating (Solution)

Official solution for the `span-memory-and-slicing-without-allocating` lab.

## What changed from the starter

- `CountWords` walks a `ReadOnlySpan<char>` with `IndexOf` and slicing. A slice is a new
  pointer and length over the same memory, so nothing is copied. `string.Split` would allocate
  an array and one string per word.
- `ParseInts` slices each field out of the line and hands the span to `int.Parse`. The caller
  owns the destination, which can be a `stackalloc` buffer.
- `Middle` uses a range on `AsSpan`. The result is a window over the original array, so
  writing to it writes to the array.
- `Sum(params ReadOnlySpan<int>)` (C# 13): the compiler builds the arguments in a stack buffer
  instead of a heap `int[]`. An existing array still works as the argument.
- `ToHex` formats into a `stackalloc` buffer and allocates only the result string. Large inputs
  fall back to a heap buffer, because the stack is small.
- `SumAsync` takes `Memory<int>` and reads `.Span` after the `await`. A `Span<int>` parameter
  or local cannot live across an `await` because it is a `ref struct`.
- `Buffer4` is an `[InlineArray(4)]` struct: four ints stored inline, convertible to a span.

## Bonus solutions

- `StartsWithZero` is an extension on `ReadOnlySpan<int>`. With the C# 14 first-class span
  conversions it can be called directly on an `int[]`, which did not bind before.
- `AsBytes` reinterprets the ints as bytes with `MemoryMarshal.AsBytes`, with no copy.

## Notes on the allocation tests

`GC.GetAllocatedBytesForCurrentThread` is measured after a warm-up call and compared with wide
margins (an order of magnitude below the naive implementation), so the tests stay stable
across runtime versions and Debug or Release builds.
