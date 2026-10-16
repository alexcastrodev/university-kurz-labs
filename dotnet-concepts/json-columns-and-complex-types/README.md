# JSON Columns and Complex Types

## Goal

Store value objects and lists in JSON columns with EF Core 10 complex types, query
inside the document, and update one member of it, in a single statement or through
change tracking. Tests run on SQLite in memory.

## Prerequisites

- EF Core basics and LINQ
- C# records
- .NET 10 SDK

## Task

`Customer` has a required `Address`, an optional `Billing` address, a list of
`Shipping` addresses and a list of `Tags`. Nothing is mapped yet. You will map the
complex types to JSON, write the queries that reach inside the documents and the
two ways of changing a value.

## Instructions

Complete the following TODOs:

- TODO-00: `Address` as a complex property in a JSON column.
- TODO-01: `Billing` (optional) the same way.
- TODO-02: `Shipping` as a complex collection in a JSON column.
- TODO-03: Filter by `Address.City`.
- TODO-04: Filter by a tag in the primitive collection.
- TODO-05: Filter with `Shipping.Any(...)`.
- TODO-06: Change a city through `SaveChanges`.
- TODO-07: Change a city for many rows with `ExecuteUpdate`.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): Update `Shipping[0].City` in bulk without creating elements for customers with an empty list.
- TODO-09 (optional): Read the stored JSON text of an address with raw SQL.
