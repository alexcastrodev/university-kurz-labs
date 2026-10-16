# Nullable Reference Types and Flow Analysis

## Goal

Tell the compiler the truth about null: annotate types, help flow analysis with attributes,
keep guard clauses where they are still needed, and see how `Nullable<T>` differs from a
nullable reference type.

## Prerequisites

- C# classes, properties and generics
- Value types versus reference types
- .NET 10 SDK

## Task

The project has `<Nullable>enable</Nullable>`, and the starter produces nullable warnings on
purpose. `User.Nickname` hides a null with `null!`, `FindUser` cannot say it may return null,
`TryGetUser` and `Trim` miss the flow-analysis attributes, and `Connection` triggers CS8618.
Nullable annotations are stored in metadata, so the tests read them with
`NullabilityInfoContext` and check the attributes with reflection. Read the compiler warnings
while you work: they are part of the exercise.

## Instructions

Complete the following TODOs:

- TODO-00: Make `User.Nickname` nullable.
- TODO-01: `DisplayName` without `!`.
- TODO-02: `FindUser` returns `User?`.
- TODO-03: `TryGetUser` with `[NotNullWhen(true)]`.
- TODO-04: `Connection.Initialize` with `[MemberNotNull]`.
- TODO-05: `Trim` with `[NotNullIfNotNull]`.
- TODO-06: `Require` keeps the run-time guard.
- TODO-07: `Compare` and lifted operators on `int?`.
- TODO-08: `FirstOrNull<T>` returns `Nullable<T>` for value types.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-09 (optional): Add `Nulls.TryFirst<T>(IEnumerable<T>, Func<T, bool>, out T value)` and
  annotate `value` with `[MaybeNullWhen(false)]`.
- TODO-10 (optional): Add `Nulls.CountBy<TKey>(IEnumerable<TKey>)` returning a
  `Dictionary<TKey, int>`, with the `notnull` constraint on `TKey`.
