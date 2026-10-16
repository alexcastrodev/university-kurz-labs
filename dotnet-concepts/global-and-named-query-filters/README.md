# Global and Named Query Filters (EF Core 10)

## Goal

Declare soft delete and multi-tenancy once in the model, switch off one rule at a
time with named filters, and apply one rule to every entity that has the flag.

## Prerequisites

- EF Core basics (`DbContext`, `DbSet`, LINQ queries)
- Lambdas and expression trees (read the code, you will build one)
- .NET 10 SDK (tests use SQLite in memory, no Docker)

## Task

`AppDbContext` has two entities with `IsDeleted` and a tenant id on `Blog`, but no
filters yet, so every query returns every row of every tenant. You will add the
filters and then the queries that deliberately bypass one of them.

## Instructions

Complete the following TODOs:

- TODO-00: Add the named `TenantFilter` on `Blog`.
- TODO-01: Add the named `SoftDeletionFilter` to every `ISoftDelete` entity with a loop.
- TODO-02: `IncludingDeleted`: ignore only the soft-delete filter.
- TODO-03: `AcrossTenants`: ignore only the tenant filter.
- TODO-04: `Undelete`: find the blog with only the soft-delete filter off.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Call `IgnoreQueryFilters(["Typo"])` and check the result. Does EF
  Core complain about a name that does not exist?
- TODO-06 (optional): Add an unnamed `HasQueryFilter` next to the named ones and read the
  exception raised when the model is built.
