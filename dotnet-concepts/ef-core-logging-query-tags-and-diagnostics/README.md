# EF Core Logging, Query Tags and Diagnostics

## Goal

Answer "what did EF Core actually send?" with `ToQueryString`, `ILogger` categories and event ids, query tags and an interceptor, and see what `EnableSensitiveDataLogging` and `EnableDetailedErrors` change.

## Prerequisites

- Basic EF Core queries
- `Microsoft.Extensions.Logging`
- .NET 10 SDK

## Task

A tiny catalog (`Product`) runs on SQLite in memory, so no Docker and no server. The tests capture what EF Core logs with a test `ILoggerProvider` that you finish first, then assert on the SQL text, the parameters in the log and the number of commands issued.

## Instructions

Complete the following TODOs:

- TODO-00: `ListLoggerProvider`: record category, level, event id and the formatted message.
- TODO-01: `ExpensiveSql`: get the SQL of a query without running it.
- TODO-02: `Expensive`: tag the query twice.
- TODO-03: `DbFactory.Create`: use the logger factory and honour the two diagnostic flags.
- TODO-04: `DbFactory.Create`: register the interceptors.
- TODO-05: `CommandCounter.ReaderExecuting`: count sync commands.
- TODO-06: `CommandCounter.ReaderExecutingAsync`: count async commands too.

Run the tests until they all pass. Read the `Executed DbCommand` message of a query with the flags off and on.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Add `ExpensiveWithCallSite` that also calls `TagWithCallSite()`. What does the comment contain?
- TODO-08 (optional): Add `DbFactory.CreateWithLogTo(connection, Action<string> sink)` that logs only `RelationalEventId.CommandExecuted` at `Information` through `LogTo`.
