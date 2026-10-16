# Extension Members and Extension Blocks (C# 14)

## Goal

Write C# 14 extension blocks: instance and static members, properties, operators, `null` and `ref` receivers, on types you do not own.

## Prerequisites

- Classic extension methods (`this` parameter)
- Generics and `IEnumerable<T>`
- a .NET 10 SDK (C# 14)

## Task

`Extensions.cs` declares the extension blocks, but every member returns a placeholder. Implement them so callers can write `names.IsEmpty`, `Money.Zero` or `counter.Increment()` as if the types declared them.

## Instructions

Complete the following TODOs:

- TODO-00: `IsEmpty`: an instance extension property on `IEnumerable<T>`.
- TODO-01: `JoinWith`: an instance extension method in the same generic block.
- TODO-02: static extension members: `IEnumerable<int>.Sequence` and `Money.Zero`.
- TODO-03: `IsBlank` on `string?` with a `null` receiver.
- TODO-04: `Increment` with a `ref int` receiver.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): `operator +` on `IEnumerable<T>`.
- TODO-06 (optional): a static extension method on the static class `Math`.
- TODO-07 (optional): `operator *` on `Money`.
