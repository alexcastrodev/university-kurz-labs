# EF Core Logging, Query Tags and Diagnostics (Solution)

Official solution for the `ef-core-logging-query-tags-and-diagnostics` lab, with the optional tasks implemented.

## What changed from the starter

- `ListLoggerProvider`: each `Log` call stores the category, level, event id and the formatted message, under a lock because EF Core can log from several threads.
- `CatalogQueries`: `ToQueryString` on a composed `IQueryable` never touches the database, and `TagWith` calls accumulate as SQL comments above the statement.
- `DbFactory.Create`: `UseLoggerFactory`, `EnableSensitiveDataLogging(flag)`, `EnableDetailedErrors(flag)` and `AddInterceptors`. The flags default to off, which is the safe setting.
- `CommandCounter`: the sync and async hooks are separate methods, so both record the command. With only the sync override, a `ToListAsync()` would pass the N+1 test without being counted.

## Why it is shaped this way

- Parameter values are hidden as `@min='?'` unless sensitive data logging is on, because they are user data. Keep it a development switch.
- The tag is part of the SQL text, so it also reaches the executed-command log and the database's own tooling.
- Without `EnableDetailedErrors`, a NULL in a non-null column fails with a generic provider message; with it, EF Core names `Product.Name`.
- The null-property test creates its own nullable table by hand, because `EnsureCreated` would make `Name` NOT NULL.

## Bonus notes

- `ExpensiveWithCallSite` adds `-- File: <path>:<line>` after the other tags.
- `CreateWithLogTo` needs no `ILoggerFactory` and no extra package: filtering by event id keeps only `CommandExecuted` (20101).

## Running

```bash
dotnet test
```
