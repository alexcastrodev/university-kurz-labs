# Interfaces: Explicit Implementation and Default Methods

## Goal

Implement the common comparison interfaces, resolve two interfaces that declare the same
member, write a default interface method, and hide a member behind an explicit implementation.

## Prerequisites

- C# interfaces and classes
- Sorting and equality in .NET (`List<T>.Sort`, `HashSet<T>`)
- .NET 10 SDK

## Task

`Person` declares `IComparable<Person>` and `IEquatable<Person>`, but the bodies are missing.
`Cowboy` implements two interfaces with one public method, `ILogger.Info` throws instead of
having a default body, and `Playlist` lacks its non-generic enumerator. Some tests use
reflection to confirm that a member is not public on the class, because that cannot be
asserted by calling it.

## Instructions

Complete the following TODOs:

- TODO-00: `Person.CompareTo` (natural order).
- TODO-01: `PersonByNameComparer` (alternative order).
- TODO-02: `Cowboy` with two explicit `Draw` implementations.
- TODO-03: `Person.Equals`, `Equals(object?)` and `GetHashCode`.
- TODO-04: The default body of `ILogger.Info`.
- TODO-05: The explicit non-generic `IEnumerable.GetEnumerator`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Add `IUnit` with a `static abstract string Symbol { get; }`, two structs
  that implement it, and `Units.Describe<T>(double)` that returns for example `"5 m"`.
- TODO-07 (optional): Add `ILeft` and `IRight` that both give a default `Name`, and a class
  `Both` that implements both and resolves the conflict with explicit implementations.
