# Value Types, Reference Types and Boxing (Solution)

Official solution for the `value-types-reference-types-and-boxing` lab.

## What changed from the starter

- `IncrementCopy` receives its own copy of the struct, so the caller never sees the change.
  `IncrementRef` receives a reference to the caller's variable, so it does.
- `Temperature` is a `readonly struct` with an `init` property. `WithCelsius` uses a `with`
  expression to build a new value instead of mutating the existing one.
- `Holder._counter` is no longer `readonly`. Calling a mutating method on a `readonly` struct
  field makes the compiler operate on a defensive copy, so the increments were silently lost
  and `IncrementTwice` returned 0.
- `BoxAll` allocates one object per element (about 24 bytes each on a 64-bit runtime).
  `UnboxAll` casts back with `(int)`, which copies the value out of the box.
- `SumOnStack` uses `stackalloc` into a `Span<int>`: the buffer lives in the stack frame, so
  there is no heap allocation and nothing for the GC to track.
- `BoxNullable` returns the `int?` as `object?`. A `Nullable<int>` without a value boxes to
  `null`, and one with a value boxes to a plain `int`, never to a `Nullable<int>`.

## Bonus solutions

- `Length(in Vector3D)` passes a `readonly struct` by readonly reference. `in` avoids the copy,
  and `readonly struct` avoids the defensive copy the compiler would otherwise add.
- `AllocatedByFormat` versus `AllocatedByInterpolation`: `string.Format("{0}", i)` boxes the int
  into an `object[]`, while the interpolated string handler formats the int without boxing.
- `IncrementFirst` uses `CollectionsMarshal.AsSpan` to get a reference to the element. The
  indexer `list[0].Increment()` does not compile (CS1612) because it returns a copy.

## Notes on the allocation tests

`GC.GetAllocatedBytesForCurrentThread` is exact for the current thread, but the tests still
use wide margins and a warm-up call, because the runtime and the JIT can add small allocations.
The .NET 10 JIT can also stack-allocate boxes that do not escape, which is why `BoxAll`
stores its boxes in an array: that makes the heap allocation unavoidable.
