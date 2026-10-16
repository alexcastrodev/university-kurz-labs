# Generics: Constraints and Variance (Solution)

Official solution for the `generics-constraints-and-variance` lab.

## What changed from the starter

- `Max<T> where T : IComparable<T>`: the constraint lets the compiler call `CompareTo`
  without a cast, and rejects types that cannot be compared.
- `Create<T> where T : class, new()`: `new()` is what makes `new T()` legal, and it must come
  last in the constraint list.
- `ByteCount<T> where T : unmanaged`: a value type with no references at any depth is the only
  kind that can be viewed as raw bytes.
- `Describe<T>` works because .NET keeps type arguments at run time: `typeof(T)` and
  `default(T)` are real, and `default(T)?.ToString()` is `null` for reference types.
- `TypeCounter<T>` is a static class with a static property. Every closed type (`TypeCounter<int>`,
  `TypeCounter<string>`) has its own copy, a cheap per-type cache.
- `CompareViaConstraint<T>` calls `value.CompareTo(other)` through the constraint. The runtime
  compiles a specialized copy for each value type, so the call is direct and allocates nothing;
  `CompareViaInterface` boxes its `int` argument on every call (24 bytes on a 64-bit runtime).
- `IProducer<out T>`: `T` only comes out of the interface, so `IProducer<Dog>` converts to
  `IProducer<Animal>`. `IConsumer<in T>`: `T` only goes in, so `IConsumer<Animal>` converts to
  `IConsumer<Dog>`. Classes such as `List<T>` are never variant.

## Bonus solutions

- `NameOf<T> where T : allows ref struct` (C# 13) accepts `ReadOnlySpan<char>`.
- Array covariance moves the type check to run time: the assignment throws
  `ArrayTypeMismatchException`. `IReadOnlyList<T>` has no way to store an element, so its
  covariance cannot fail.

## Running

```bash
dotnet test
```
