# Garbage Collector, Generations and the LOH

## Goal

Observe generations, promotion, the large object heap, weak references and pooling with real measurements instead of guesses.

## Prerequisites

- Reference types and the managed heap
- Finalizers and `WeakReference`
- .NET 10 SDK

## Task

`Heap`, `Weak`, `Resurrector` and `Buffers` are stubbed. Each TODO asks you to run a small experiment against the real runtime and return what it reports. The tests never depend on exact heap sizes or timing: they only check the behavior the concept teaches. All GC-sensitive tests live in one class with parallelization disabled, because the GC is process-wide.

## Instructions

Complete the following TODOs:

- TODO-00: `GenerationOfNewByteArray`: large arrays (85,000 bytes or more) are born in generation 2, small ones are not.
- TODO-01: `GenerationsAfterCollections`: a survivor is promoted one generation per collection.
- TODO-02: `CollectionsTriggeredBy`: force a collection of a generation and measure it with `GC.CollectionCount`, rejecting generations above `GC.MaxGeneration`.
- TODO-03: `Weak.Of` and `Weak.IsAlive`: a weak reference does not keep its target alive.
- TODO-04: `Resurrector`: a finalizer that resurrects the object clears a short weak reference but not a long one.
- TODO-05: `UsePooledArray`: rent and return a buffer with `ArrayPool<byte>.Shared` instead of allocating a new LOH array per call.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
dotnet test -c Release
```

## Bonus (Optional)

- TODO-06 (optional): `RequestLohCompaction`: ask for a one-time LOH compaction and see the setting reset itself.
- TODO-07 (optional): `GenerationOfPinnedArray`: allocate on the pinned object heap and check its generation.
