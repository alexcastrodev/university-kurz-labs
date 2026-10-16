# Blazor Data Access and Streaming Rendering

## Goal

Read a database from a Blazor Web App the safe way: one short-lived `DbContext` per
operation through `IDbContextFactory<T>`, read-only queries without change tracking,
and a static SSR page that uses `[StreamRendering]` so the browser gets a placeholder
first and the data as soon as the query finishes.

## Prerequisites

- C# and `async`/`await`
- Basic EF Core (`DbContext`, `DbSet`, LINQ)
- Razor components and the component lifecycle (`OnInitializedAsync`)
- Dependency injection lifetimes (scoped vs singleton)
- .NET 10 SDK

## Task

A small Blazor Web App lists products from an in-memory SQLite database (a named
shared-cache database; a "keeper" connection registered in DI keeps it alive for the
lifetime of the host). Two pages show the same list: `/products` should stream, and
`/products-plain` is the contrast and waits for the data before sending anything.

To make slowness controllable without any delay, the pages await an `ILoadGate` before
querying. In production it completes immediately (`ImmediateGate`). The tests replace
it with a gate they hold closed, so a "slow query" lasts exactly as long as the test
wants. The tests run the whole app in memory with `WebApplicationFactory<Program>`.

## Instructions

Complete the following TODOs:

- TODO-00: In `Program.cs`, register `IDbContextFactory<ShopContext>`
  (`AddDbContextFactory` with `UseSqlite(connectionString)`) and `IProductService` as
  `DbProductService`.
- TODO-01: Implement `DbProductService.ListAsync`: create a context from the factory for
  this call only (and dispose it), use `AsNoTracking`, order by name.
- TODO-02: Implement `DbProductService.CountConcurrentlyAsync`: run the requested number
  of overlapping queries with `Task.Run` and `Task.WhenAll`, each one creating its own
  context. One `DbContext` is not thread safe, which is exactly why each query needs its
  own.
- TODO-03: In `Products.razor`, load the list in `OnInitializedAsync`: await
  `Gate.WaitAsync()`, then `items = await Service.ListAsync()`.
- TODO-04: Put `[StreamRendering]` on `Products.razor` (`@attribute [StreamRendering]`).
  The first flush must contain `Loading...`, and the data must arrive later in the same
  response as a streaming update (`blazor-ssr` markup).
- TODO-05: In `Program.cs`, expose `IProductService.ListAsync` as `GET /api/products`.
  This is the shape a WebAssembly client would call, since it cannot reach the database.

Run the tests until they all pass.

Note: the streaming test has a 15 second watchdog. If TODO-04 is missing, the headers
never arrive until the data is ready, so the test fails when the watchdog fires instead
of hanging forever. A correct solution never waits for it.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Implement `DbProductService.RenameAsync`: load the product, change
  the name and save inside one short-lived context. Return `false` for an unknown id.
- TODO-07 (optional, no test): Experiment in a scratch test or console app. Inject one
  `ShopContext` directly and run eight overlapping `CountAsync` calls with `Task.Run`.
  You will usually see `InvalidOperationException: A second operation was started on this
  context instance...`. SQLite finishes async calls synchronously, so it is not
  guaranteed, which is why this is not an automated test. Then switch to the factory and
  watch it succeed. What does `AsNoTracking` change here, and what does it not change?

What the tests cannot show: in a real browser `blazor.web.js` applies the streamed update
in place, and an interactive component (Interactive Server) prerenders and so queries
twice unless you use `[PersistentState]`. Both need a browser or an interactive render
mode, so they are only described here.
