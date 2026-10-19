# EF Core, ADO.NET, Dapper and NoSQL

## Goal

Run the same small domain through three data-access layers (plain ADO.NET, Dapper
and EF Core) over one in-memory SQLite database, feel what each one costs and buys
you, mix two of them in one transaction, and finish with a tiny document store to
see what you give up and gain when the shape is flexible. Everything runs inside the
test process: no server, no Docker, no network.

## Prerequisites

- C# and LINQ basics
- Basic SQL (SELECT, JOIN, INSERT, transactions)
- EF Core basics
- .NET 10 SDK

## Task

`Schema.CreateAndSeed` already creates `Categories`, `Products` (the price column is
called `unit_price`, the property is `UnitPrice`) and a `Documents` table. Every test
opens one in-memory connection and keeps it open, so the database lives as long as
the test. The data-access classes are fully implemented in this solution.
You will write the same queries with each tool and then combine them.

## Instructions

Complete the following TODOs:

- TODO-00: `AdoNetProducts.FindByCategory` and `Add`: parameterized `SqliteCommand`, hand-written mapping, hostile input stored literally.
- TODO-01: `AdoNetProducts.AddAll`: one transaction, nothing is stored when one row fails.
- TODO-02: `DapperProducts.FindByCategory`: automatic mapping, including the `unit_price` column.
- TODO-03: `DapperProducts.CategoriesWithProducts`: one join query instead of N+1.
- TODO-04: `EfProducts.ListReadOnly`: the same query in LINQ, without change tracking.
- TODO-05: `EfProducts.RaisePrices`: Dapper inside an EF Core transaction, on the same connection.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `DocumentStore`: put, get and query by JSON path on a key/document table. Notice that nothing stops a document from pointing to a category that does not exist.
- TODO-07 (optional): `DapperProducts.AddAll`: insert a whole list with a single `Execute` call.
