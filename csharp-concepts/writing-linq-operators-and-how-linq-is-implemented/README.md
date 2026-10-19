# Writing LINQ Operators and How LINQ Is Implemented

## Goal

Write your own LINQ operators as iterator methods, validate arguments eagerly, release the source when a consumer stops early, and special-case `ICollection<T>` / `IList<T>` the way `System.Linq` does.

## Prerequisites

- Extension methods and generics
- `yield return` and `IEnumerable<T>` / `IEnumerator<T>`
- Deferred execution (see the LINQ deferred execution lab)
- .NET 10 SDK

## Task

`MyLinq` is stubbed. The tests feed your operators from a `TrackedSource` (an iterator that records how many items were pulled, how many enumerations started and how many enumerators were disposed) and from a `ListSpy` (an `IList<int>` that counts the enumerators requested). Laziness and fast paths are observed through those counters, never through timing.

## Instructions

Complete the following TODOs:

- TODO-00: `MyWhere` and `MySelect`: validate in a public method, iterate in a private `yield` method, one pull at a time.
- TODO-01: `MyTake`: stop without an extra `MoveNext`, do not touch the source for 0, dispose on early exit.
- TODO-02: `MyDistinctBy`: stream the elements and keep the "seen" set per enumeration.
- TODO-03: `MyChunk`: buffer only one chunk, a shorter last chunk, independent arrays.
- TODO-04: `MyCount` and `MyElementAt`: use `ICollection<T>.Count` and the `IList<T>` indexer when the source offers them.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Write `MyZip` that stops at the shorter sequence. When the first sequence ends, it must not pull from the second one. Both enumerators must be disposed.
- TODO-06 (optional): Write `MyTryGetNonEnumeratedCount` that answers for `ICollection<T>` and returns `false` for iterators, like `Enumerable.TryGetNonEnumeratedCount`. Why can `MyWhere` never answer?
