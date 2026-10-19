# LINQ Operator Families and Query Syntax

## Goal

Pick LINQ operators by family (element, quantifier, aggregation, ordering, partitioning, type filtering) and write queries in query syntax with `let` and `into`.

## Prerequisites

- Deferred execution in LINQ
- Lambdas and `IEnumerable<T>`
- .NET 10 SDK

## Task

`Queries` is stubbed. Each method is small, but the right operator changes what happens on empty input, on duplicates and under another culture.

## Instructions

Complete the following TODOs in `Queries`:

- TODO-00: `FindByEmail`: an element operator that returns null for zero matches and throws for more than one.
- TODO-01: `AllInStock` and `MostExpensive`: the vacuous truth of `All` on an empty sequence, and `MaxBy` returning the item.
- TODO-02: `Ranking` and `SortedKeys`: `ThenBy` for secondary keys, and `Order()` with an ordinal comparer so the result does not depend on the culture.
- TODO-03: `Page` and `BatchSums`: partitioning with `Skip`, `Take` and `Chunk`.
- TODO-04: `IntsOnly` and `AllInts`: `OfType` filters, `Cast` asserts.
- TODO-05: `Describe` and `RepeatedLengths`: query syntax with `let` and `group ... by ... into`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Run `from w in words.AsQueryable() let len = w.Length where len > 3 select w` and print `.Expression`. Find the transparent identifier and count how many `Select` calls the `let` added.
- TODO-07 (optional): Write a `Box<T>` with `Where` and `Select` methods and use it in `from x in box where x > 10 select x * 2`. Why does query syntax compile for a type that is not a sequence?
