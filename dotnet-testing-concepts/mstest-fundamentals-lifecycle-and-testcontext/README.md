# MSTest 4 Fundamentals: Lifecycle, TestContext and Parallelism

## Goal

A small shop (an `Inventory` that reserves stock and a `Cart` that checks out through a payment gateway) is
already fully implemented. Your job is to write the MSTest tests that prove it works, and on the way find out
how MSTest really runs them: in which order the lifecycle hooks run, why every test gets a new instance of its
class, what goes wrong when tests share `static` state while running in parallel, and how an exception
assertion can pass for the wrong reason.

## Prerequisites

- C# basics: classes, exceptions, `async`/`await`
- .NET 10 SDK

## Task

Read `src/Shop` first (`Inventory.cs`, `Cart.cs`). Then open `tests/Shop.Tests`. Every test method is already
declared with a hint, and fails with a `TODO-NN` message until you write it. The payment fakes in
`TestDoubles.cs` are only shells for now.

Two files are not empty. `StockIsolationTests` already has tests, and they are broken by design: run them and
see what shared state does. `CartTests.AddingZeroUnitsIsRejected` already has an assertion that passes, but
not for the reason its name claims.

`MSTestSettings.cs` turns on method-level parallelism: tests from every class run at the same time.

## Instructions

Complete the following TODOs in `tests/Shop.Tests`:

- TODO-00: `LifecycleOrderTests`: add every lifecycle hook, have each one write to the log, and prove the
  whole order: the test checks what ran before it, `[AssemblyCleanup]` checks everything.
- TODO-01: `InventoryTests`: reservations, the exception and its properties when stock runs out, unknown
  products, and the rejected quantities with `[DataRow]`.
- TODO-02: `StockIsolationTests`: run the shared-state tests, watch them fail, then give each test its own
  inventory.
- TODO-03: `CartTests`: quantities, total, removal, and fix the test that passes for the wrong reason.
- TODO-04: `TestDoubles.cs`: write `ApprovingGateway`, `DecliningGateway` and `HangingGateway`.
- TODO-05: `CheckoutTests`: the `TestContext` through the constructor, its `CancellationToken`, and the
  approved, declined, cancelled and empty checkouts.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

To see what the tests write with `TestContext.WriteLine`:

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Bonus (Optional)

- TODO-06 (optional): Tag `CheckoutTests` with `[TestCategory("Checkout")]`. Run only those tests with
  `dotnet test --filter "TestCategory=Checkout"`, and everything else with `--filter "TestCategory!=Checkout"`.
- TODO-07 (optional): Mutation check. Change the production code on purpose, one change at a time, and see
  whether your suite notices: delete the stock check in `Inventory.Reserve`, make `Cart.Add` overwrite the
  quantity instead of adding to it, and change the `catch` in `Cart.CheckoutAsync` to
  `catch (PaymentDeclinedException)`. Each change should turn at least one of your tests red. A change that
  leaves everything green means a missing test.
