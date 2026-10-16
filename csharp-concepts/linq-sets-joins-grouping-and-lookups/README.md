# LINQ: Sets, Joins, Grouping and Lookups

## Goal

Use `Join`, `GroupJoin`, the .NET 10 `LeftJoin`, `ToLookup`, `ExceptBy` and `GroupBy` on in-memory data and observe what each one keeps or drops.

## Prerequisites

- LINQ basics (`Where`, `Select`)
- Tuples and records
- .NET 10 SDK

## Task

`Queries` has six methods stubbed over `Customer` and `Order`. One order has no customer (`CustomerId` is null) and one customer has no orders, which is what makes the join flavors differ.

## Instructions

Complete the following TODOs:

- TODO-00: `InnerJoin`: `Join` with matching key types (`int` versus `int?`).
- TODO-01: `LeftJoin`: the .NET 10 `LeftJoin` operator.
- TODO-02: `CountOrders`: `GroupJoin` keeps customers with zero orders.
- TODO-03: `OrdersByCustomer`: `ToLookup` is immediate and returns empty for missing keys.
- TODO-04: `NewCustomers`: `ExceptBy` with a case-insensitive comparer.
- TODO-05: `TotalPerCustomer`: `GroupBy` with a result materialized into a list.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Implement `LeftJoin` again with `GroupJoin`, `SelectMany` and `DefaultIfEmpty`, the pre-.NET 10 way, and compare the code.
- TODO-07 (optional): Replace `ToLookup` with `GroupBy(...).ToDictionary(...)` and note what happens on a missing key.
