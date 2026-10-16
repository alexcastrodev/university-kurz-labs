# Expression Trees and IQueryable Providers

## Goal

See a lambda as data: build one by hand, rewrite one with an `ExpressionVisitor`, watch an `IQueryProvider` receive the tree of every operator, and keep captured values as variables instead of constants.

## Prerequisites

- LINQ with `IEnumerable<T>` and `IQueryable<T>`
- Lambdas and closures
- .NET 10 SDK

## Task

An in-memory list of `Order` is queried through a small logging provider that delegates to the in-memory one. Every class is stubbed; the tests only use `AsQueryable()`, no database.

## Instructions

Complete the following TODOs:

- TODO-00: `Builders.TotalGreaterThan`: build `o => o.Total > value` without a lambda literal.
- TODO-01: `Builders.TotalAboveCaptured`: keep the variable in the tree, not its value.
- TODO-02: `ReplaceParameter`: swap one parameter for another.
- TODO-03: `Or`: combine two predicates into one lambda.
- TODO-04: `MemberCollector`: list the members a lambda reads.
- TODO-05: `LoggingProvider.CreateQuery`: log the operator and keep logging the next one.
- TODO-06: `LoggingProvider.Execute`: log the whole tree and delegate.
- TODO-07: `OrderQueries.Filter`: optional AND filters by chaining `Where`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Add `And<T>` next to `Or<T>`.
- TODO-09 (optional): Add `OrderQueries.OrderByProperty(source, "Total", descending)` building the `OrderBy` call by hand and sending it to `source.Provider.CreateQuery`.
- TODO-10 (optional): Try to write a statement lambda, a `?.`, an `is` pattern and a switch expression as `Expression<Func<Order, ...>>`. Note the compiler error codes (CS0834, CS8072, CS8122, CS8514).
