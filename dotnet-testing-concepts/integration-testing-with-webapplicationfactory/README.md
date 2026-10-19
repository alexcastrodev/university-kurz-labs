# Integration Testing ASP.NET Core with WebApplicationFactory

## Goal

The orders API in `src/Orders.Api` is already fully implemented: a minimal API with validation,
ProblemDetails, an inventory service it calls over HTTP, an in-memory store and a redirect. Your job is to
write the MSTest integration tests that start it in memory with `WebApplicationFactory<Program>`, replace
what the tests cannot reach, and assert on what a client really receives. Some of these tests pass for the
wrong reason when written the obvious way; the hints tell you where.

## Prerequisites

- Minimal APIs and the `Program.cs` pipeline
- MSTest lifecycle: `[TestInitialize]`, `[ClassInitialize]`, `[ClassCleanup]`, `TestContext`
- .NET 10 SDK

## Task

Read `src/Orders.Api/Program.cs` and `src/Orders.Api/Orders/OrderEndpoints.cs`, then open
`tests/Orders.Api.Tests`. Every test method is already declared with a hint and fails with
`TODO-NN: write this test` until you write it.

Three things in the API are there on purpose:

- `Program.cs` reads `Orders:MaxQuantity` **before** `builder.Build()`.
- `OrderStore` is a **singleton**: one list for the life of the host.
- `GET /orders/latest` answers with a **302**, and the default test client follows redirects.

## Instructions

Complete the following TODOs:

- TODO-00: `FakeInventory`: an in-memory implementation of `IInventoryClient`.
- TODO-01: `OrdersApiFactory.ConfigureWebHost`: lower the order limit to 3 and swap the inventory for the fake.
  Only one of the two obvious ways to override configuration reaches `Program.cs` in time.
- TODO-02: `OrdersApiTests`: one factory per class with `[ClassInitialize]`/`[ClassCleanup]`, and a reset of the
  shared state before each test.
- TODO-03: 201 with `Location`, a `GetFromJsonAsync` round trip, and a 404 with ProblemDetails.
- TODO-04: the configured limit, tested at the boundary (just above it and exactly at it).
- TODO-05: 409 when the fake inventory has too little stock.
- TODO-06: the list endpoint returns exactly the orders of this test.
- TODO-07: the 302 of `/orders/latest`, with a client that does not follow redirects.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): `ConfigurationOverrideTests`: prove that `ConfigureAppConfiguration` arrives too late for
  a value read before `Build`, and that `IConfiguration` still shows it afterwards.
- TODO-09 (optional): `KestrelTests`: start the same factory on a real socket with `UseKestrel(0)` (new in .NET 10).
- TODO-10 (optional): Mutation check. Break the production code on purpose, one change at a time, and see which
  test turns red: build `OrderRules` with a hard-coded `10`, return `Results.Ok(order)` from `/orders/latest`,
  make `OrderStore.Latest` return the oldest order, skip the stock check, return `Results.NotFound()` without a
  body. Each change should turn at least one test red. Then delete your reset from TODO-02 and run the class.
