# Testing EF Core Queries, Concurrency and Migrations

## Goal

A multi-tenant ordering module (`Order`, `OrderLine`, `OrderService`, `OrderQueries`, two real migrations)
is already fully implemented on PostgreSQL. Your job is to write MSTest tests for the parts a fake database
cannot see: how queries translate to SQL, the tenant and soft-delete query filters, optimistic concurrency
on `xmin`, a unique index turned into a domain error, the migrations themselves, and the number of SQL
commands a query sends.

## Prerequisites

- Testing Against a Real Database with Testcontainers (the concept and its lab)
- EF Core: query filters, concurrency tokens, migrations
- .NET 10 SDK
- **Docker running locally.** The tests start `postgres:18-alpine` with Testcontainers.

## Task

Read `src/Ordering` first, including `Migrations/`. Then open `tests/Ordering.Tests`. The container,
the database per class and the reset (`TRUNCATE ... RESTART IDENTITY`) are set up the same way as in the
previous lab. Every test is declared with a hint and fails with `TODO-NN` until you write it.

Three traps are waiting:

- An N+1 query returns the right numbers. Only a test that counts commands sees it.
- A model change without a migration builds and runs fine until `MigrateAsync` meets a real database.
- A concurrency "conflict" simulated with one `DbContext` never conflicts, with or without a token.

Assert with a new context every time (`_database.NewContext(...)`), so the assertion reads the database
and not what a context still tracks.

## Instructions

Complete the following TODOs in `tests/Ordering.Tests`:

- TODO-00: `PostgresContainer`: start one container in `[AssemblyInitialize]`, dispose it in `[AssemblyCleanup]`.
- TODO-01: `TestDatabase.CreateAsync`: create the database and apply the migrations with `MigrateAsync`.
- TODO-02: `CommandCounter`: record the text of every query EF Core executes.
- TODO-03: `QueryTranslationTests`: case-insensitive search, summaries computed in SQL, an untranslatable query.
- TODO-04: `QueryFilterTests`: tenant isolation, soft delete, and ignoring only the soft-delete filter.
- TODO-05: `ConcurrencyTests`: a real conflict with two contexts, the one-context learning test, `xmin` moving.
- TODO-06: `UniqueViolationTests`: a taken number is a `DuplicateOrderNumberException`, per tenant.
- TODO-07: `MigrationTests`: every migration applies to an empty database, and the model has no pending changes.
- TODO-08: `NPlusOneTests`: the summaries take one command for 1 order and for 5.

Run the tests until they all pass.

## Running the Lab

Start Docker, then from the lab directory:

```bash
dotnet test
```

If `~/.testcontainers.properties` was written by Testcontainers for Java (`docker.host=unix\:///...`),
set `DOCKER_HOST=unix:///var/run/docker.sock` for the run, as in the previous lab.

## Bonus (Optional)

- TODO-09 (optional): Model drift. Add `public string? Note { get; private set; }` to `Order` and run the
  tests: every test fails, because `MigrateAsync` refuses a model with pending changes (EF Core 9 and later)
  and `HasPendingModelChanges()` is true. Run `dotnet ef migrations add AddNote` in `src/Ordering` (install
  the tool with `dotnet tool install --global dotnet-ef` if needed) and the suite is green again.
  Remove the property and the migration afterwards.
- TODO-10 (optional): Mutation check. One at a time, then revert: rewrite `SummariesAsync` as a loop with
  one query per order; remove the `Tenant` query filter; call `IgnoreQueryFilters()` with no names in
  `DeletedNumbersAsync`; remove the `catch` in `PlaceAsync`; replace `EF.Functions.ILike` with `Contains`.
  Each change should turn at least one test red. Then replace `IsRowVersion()` with
  `order.Ignore(o => o.Version)` and add a migration for it: only the two-context test and the version test
  fail, and the one-context test stays green.
