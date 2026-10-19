# MSTest 4 Fundamentals: Lifecycle, TestContext and Parallelism (Solution)

Official solution for the `mstest-fundamentals-lifecycle-and-testcontext` lab.

## What changed from the starter

- **`LifecycleOrderTests`** has every hook, and each one appends its name to a static log. The test asserts
  the first five entries (`AssemblyInitialize`, `ClassInitialize`, `Constructor`, `TestInitialize`, `Test`).
  Only `[AssemblyCleanup]`, the last hook, can see the rest (`TestCleanup`, `Dispose`, `ClassCleanup`), so it
  asserts the whole sequence. A failing assertion in `[AssemblyCleanup]` is reported as an extra failed
  result: with `dotnet test` (VSTest) it carries the name of whichever test finished last, so read the
  message, not the test name. Assembly hooks belong to the assembly, so they run even when a filter leaves
  this class out; the cleanup returns early when `"Test"` is not in the log, or every filtered run would fail.
  The class-level and assembly-level hooks are `public static`;
  `[AssemblyInitialize]` and `[ClassInitialize]` must take a `TestContext`, the cleanups may omit it.
- **`InventoryTests`** uses `Assert.ThrowsExactly<T>`, which returns the exception, so the test can check
  `Requested` and `Available`. A second test checks that a failed reservation leaves the stock as it was.
  The rejected quantities are one test with two `[DataRow]` rows.
- **`StockIsolationTests`** used one `static` inventory stocked in `[ClassInitialize]`. Each test takes 6 of
  10 apples, so the second one to run finds 4 and fails, and which one that is changes from run to run. With
  `MethodLevel` parallelism two tests can also write to the same `Dictionary` at once. The fix is a
  `readonly` instance field set by the constructor: MSTest creates a new instance of the class for every
  test, and for every data row.
- **`CartTests.AddingZeroUnitsIsRejected`** was `Assert.Throws<ArgumentException>(() => _cart.Add("Apple", 0))`.
  SKUs are case-sensitive, so `"Apple"` is unknown, `Reserve` throws `UnknownProductException`, and that
  derives from `ArgumentException`. `Assert.Throws<T>` accepts derived types, so the test stayed green with
  the quantity check deleted. The fix names the exact type (`ThrowsExactly<ArgumentOutOfRangeException>`)
  and uses a SKU the inventory knows.
- **`CheckoutTests`** receives the `TestContext` in its constructor, passes `TestContext.CancellationToken`
  to the code under test and writes the receipt with `TestContext.WriteLine`. The cancellation test links a
  `CancellationTokenSource` to that token and cancels it after 50 ms. `Task.Delay` then throws
  `TaskCanceledException`, which derives from `OperationCanceledException`: here "it was cancelled" is the
  contract, so `Assert.ThrowsAsync<OperationCanceledException>` (derived types accepted) is the right
  assertion, and `ThrowsExactlyAsync<OperationCanceledException>` fails. `[Timeout(5000, CooperativeCancellation = true)]`
  is the safety net if the code ever ignores the token.
- **The fakes** are hand-written. `HangingGateway` awaits `Task.Delay(Timeout.Infinite, cancellationToken)`,
  so it finishes only when the token is cancelled.

## Bonus notes

- `dotnet test --filter "TestCategory=Checkout"` runs the 4 checkout tests; `--filter "TestCategory!=Checkout"`
  runs the other 15.
- Mutation check, run against this solution. Each change below to `src/Shop` was tried on its own:

| Change to the production code | Tests that fail |
| --- | --- |
| `Inventory.Reserve` skips the stock check | `ReservingMoreThanIsAvailableSaysHowManyAreLeft`, `AFailedReservationLeavesTheStockUnchanged` |
| `Inventory.Reserve` skips the quantity check | `ReservingANonPositiveQuantityIsRejected` (both rows), `AddingZeroUnitsIsRejected` |
| `Inventory.Reserve` subtracts before checking the stock | `AFailedReservationLeavesTheStockUnchanged` |
| `Cart.Add` overwrites the quantity instead of adding | `AddingTheSameProductTwiceAddsUpTheQuantity` |
| `Cart.Remove` does not release the stock | `RemovingALineGivesTheStockBack` |
| `CheckoutAsync` catches only `PaymentDeclinedException` | `ACancelledCheckoutGivesTheStockBack` |
| `CheckoutAsync` keeps the lines after a successful payment | `AnApprovedPaymentProducesAReceiptAndEmptiesTheCart` |

- The starter's version of `AddingZeroUnitsIsRejected` stays green when the quantity check is deleted. That
  is the whole point of the TODO.
- Changing the tests instead of the code is also instructive: make the inventory in `StockIsolationTests`
  `static` again and three tests fail (one of the "six apples" tests, which one varies, and both data rows).
  Swap `Dispose` and `ClassCleanup` in `ExpectedOrder` and the `[AssemblyCleanup]` assertion fails the run.
