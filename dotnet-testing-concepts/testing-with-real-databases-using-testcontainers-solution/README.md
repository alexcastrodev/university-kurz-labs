# Testing Against a Real Database with Testcontainers (Solution)

Official solution for the `testing-with-real-databases-using-testcontainers` lab.

Requires Docker running locally: the tests start `postgres:18-alpine` with Testcontainers. Run with
`dotnet test` from this directory (11 tests, about 3 seconds once the image is pulled).

## What changed from the starter

- **`PostgresContainer`** starts one container in `[AssemblyInitialize]` and disposes it in
  `[AssemblyCleanup]`. `new PostgreSqlBuilder("postgres:18-alpine")` pins the image instead of trusting
  a default. Both methods are `public static` in a `[TestClass]`; `AssemblyInitialize` must take the
  `TestContext`, and its `CancellationToken` is passed to `StartAsync`. Ryuk, the reaper container that
  Testcontainers starts next to it, removes the container if the process dies before the cleanup runs.
- **One database per test class.** `CreateDatabaseAsync` runs `CREATE DATABASE` inside the shared
  container, and `TestDatabase.CreateAsync` adds the schema with `EnsureCreatedAsync`. A database costs
  milliseconds; a container costs seconds. `MSTestSettings.cs` uses `ExecutionScope.ClassLevel`, so
  classes run in parallel (each on its own database) and the methods of one class run in sequence.
- **`RollbackIsolationTests`** opens a context and a transaction in `[TestInitialize]` and disposes the
  context in `DisposeAsync` without committing, which rolls everything back. Inside a user transaction,
  EF Core's `SaveChanges` uses a savepoint, so even a failed save leaves the transaction usable. The
  read-back calls `ChangeTracker.Clear()` first; without it the query returns the instance the context
  already tracks, and the test passes even when nothing was saved.
- **`RespawnIsolationTests`** lets the code commit and resets before each test. The `Respawner` is built
  once per class (it reads the schema), and `ResetAsync` gets an open `NpgsqlConnection`. On PostgreSQL
  Respawn runs one `truncate table ... cascade` for the tables it found. Every assertion uses a new
  context, so it reads what was committed.
- **Database errors are checked by code, not by message.** A duplicate SKU arrives as a
  `DbUpdateException` whose `InnerException` is a `PostgresException` with `SqlState` equal to
  `PostgresErrorCodes.UniqueViolation` (`23505`); a SKU longer than `varchar(20)` gives
  `StringDataRightTruncation` (`22001`).
- **`FakeProviderTests`** are learning tests. The InMemory provider stores the same SKU twice, and SQLite
  stores a 21-character SKU, because neither checks the unique index or the column length. PostgreSQL
  rejects both, as the Respawn tests show.
- **`TruncateIsolationTests`** (TODO-07) resets with `TRUNCATE "Products" RESTART IDENTITY CASCADE`. You
  name the tables, but the identity sequence restarts, so each test's first product gets `Id == 1`. With
  Respawn's default options the id keeps growing across tests.

## Bonus notes

Mutation check, run against this solution. Each change below was tried on its own and then reverted:

| Change | Tests that fail |
| --- | --- |
| `ReceiveAsync` does not call `SaveChangesAsync` | `ReceivedStockIsStoredInTheDatabase`, `ReceivedStockIsCommitted`, `AReservationBeyondTheStockIsRejectedAndChangesNothing`, `TheLowStockReportListsOnlyProductsUnderTheThreshold` |
| `.IsUnique()` removed from the SKU index | `ASecondProductWithTheSameSkuIsRejected` |
| `Product.Reserve` skips the stock check | `AReservationBeyondTheStockIsRejectedAndChangesNothing` |
| `HasMaxLength` removed from the SKU | `ASkuLongerThanTheColumnIsRejected` |
| (test code) `ResetAsync` does not call Respawn | `ReceivedStockIsCommitted`, `ASecondProductWithTheSameSkuIsRejected` (the empty-catalog test ran first and stayed green) |

Two results are the point of the lab:

- With the first change and the `ChangeTracker.Clear()` line removed from
  `ReceivedStockIsStoredInTheDatabase`, that test is green. Same-context assertions do not see a missing save.
- The `.IsUnique()` and `HasMaxLength` changes do not affect `FakeProviderTests`: those fakes never
  checked the constraint in the first place.

`dotnet test --filter "TestCategory=Integration"` runs only the PostgreSQL classes.
