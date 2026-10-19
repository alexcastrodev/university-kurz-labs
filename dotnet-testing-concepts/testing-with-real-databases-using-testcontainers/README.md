# Testing Against a Real Database with Testcontainers

## Goal

A small inventory module (`Product`, `StockService`, `InventoryDbContext` on PostgreSQL) is already fully
implemented. Your job is to write MSTest tests that run against a real PostgreSQL started by Testcontainers:
one container for the whole run, one database per test class, and two ways of keeping tests from seeing
each other's rows (a transaction per test, and a reset with Respawn). Two learning tests show what the
EF Core InMemory provider and SQLite let through.

## Prerequisites

- MSTest Fundamentals: Lifecycle and TestContext (`[AssemblyInitialize]`, `[ClassInitialize]`, `TestContext`)
- EF Core basics: `DbContext`, `SaveChangesAsync`, change tracking
- .NET 10 SDK
- **Docker running locally.** Testcontainers talks to the Docker daemon to start `postgres:18-alpine`
  (the first run pulls the image). Without Docker every test fails in `[AssemblyInitialize]`.

## Task

Read `src/Inventory` first. Then open `tests/Inventory.Tests`. Every test method and lifecycle method is
already declared with a hint, and fails with `TODO-NN` until you write it.

This is not about copying the article. Three traps are waiting:

- A test that reads back through the same `DbContext` that wrote gets the tracked instance, not the row.
  It stays green even if `SaveChangesAsync` is never called.
- A test that counts rows passes when it runs alone and fails when another test left rows behind.
  Run the whole class, not one test.
- A test can only prove the database rejects bad data on a database that checks it. The InMemory provider
  and SQLite accept data that PostgreSQL refuses.

## Instructions

Complete the following TODOs in `tests/Inventory.Tests`:

- TODO-00: `PostgresContainer`: start one `PostgreSqlContainer` in `[AssemblyInitialize]` and dispose it
  in `[AssemblyCleanup]`.
- TODO-01: `TestDatabase.CreateAsync`: create the database in the container and its schema with
  `EnsureCreatedAsync`.
- TODO-02: `RollbackIsolationTests`: one database for the class, a new context with an open transaction
  per test, disposed (rolled back) after it.
- TODO-03: `RollbackIsolationTests`: the three tests, reading back after `ChangeTracker.Clear()`.
- TODO-04: `RespawnIsolationTests`: one database and one `Respawner` for the class, a reset before each test.
- TODO-05: `RespawnIsolationTests`: the four tests, asserting with a new context.
- TODO-06: `FakeProviderTests`: the two learning tests on InMemory and SQLite.

Run the tests until they all pass.

## Running the Lab

Start Docker, then from the lab directory:

```bash
dotnet test
```

The first run pulls the PostgreSQL image. After that, the container starts in a few seconds and the whole
suite runs in about the same time. `MSTestSettings.cs` runs test classes in parallel and the methods of a
class one after another, because each class owns a database and its methods share it.

If you also use Testcontainers for Java, check `~/.testcontainers.properties`. A line written by the Java
library, such as `docker.host=unix\:///var/run/docker.sock`, keeps the backslash when .NET reads it, and the
start fails with "This operation is not supported for a relative URI". Set `DOCKER_HOST` (for example
`DOCKER_HOST=unix:///var/run/docker.sock dotnet test`) or remove the backslash.

## Bonus (Optional)

- TODO-07 (optional): Add a `TruncateIsolationTests` class that resets with
  `TRUNCATE "Products" RESTART IDENTITY CASCADE` in `[TestInitialize]`, and a test with two `[DataRow]`
  rows that expects the first product of each test to get `Id == 1`. Respawn also truncates on PostgreSQL,
  but it does not restart the sequences by default.
- TODO-08 (optional): Mutation check. Change the production code on purpose and see whether your suite
  notices: remove the `SaveChangesAsync` call from `ReceiveAsync`, remove `.IsUnique()` from the SKU index,
  remove the stock check in `Product.Reserve`, remove `HasMaxLength` from the SKU. Each change should turn
  at least one test red. Then put the first change back and replace `ChangeTracker.Clear()` with nothing:
  the rollback test turns green again, for the wrong reason.
