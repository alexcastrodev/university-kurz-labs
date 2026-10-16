# Generated Values, Defaults and Computed Columns

## Goal

Tell EF Core which values the database produces (defaults, clock, computed columns,
triggers) so it leaves them out of `INSERT`, reads them back, and does not confuse
"not set" with an intentional `0`. Tests run on SQLite in memory.

## Prerequisites

- EF Core basics (`DbContext`, `SaveChanges`)
- A little SQL (`DEFAULT`, computed columns, triggers)
- .NET 10 SDK

## Task

`Account` has a `Status`, `Credits`, `CreatedAt`, `UpdatedAt` and `FullName`, and the
database should fill them. `ShopDatabase.Create` already creates the schema and a trigger
that sets `UpdatedAt`. The model says nothing yet, so every value is sent by the
application or stays empty.

## SQLite note

The concept also covers sequences (`HasSequence`, `NEXT VALUE FOR`) and SQL Server DDL.
SQLite has no sequences, so this lab leaves them out; the SQL Server script can be
generated with `UseSqlServer(...)` and `Database.GenerateCreateScript()` without a server.
Row versions are covered by the transactions concept, so there is no row version here.

## Instructions

Complete the following TODOs:

- TODO-00: `Status` default value `"Active"`.
- TODO-01: `Credits` default 10 with the right sentinel.
- TODO-02: `CreatedAt` with `HasDefaultValueSql("CURRENT_TIMESTAMP")`.
- TODO-03: `FullName` as a stored computed column.
- TODO-04: `UpdatedAt` generated on add or update, read back after the trigger.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): A client-side `GuidValueGenerator` for `Ticket.Ref`.
- TODO-06 (optional): A virtual (not stored) computed column for `Initials`.
