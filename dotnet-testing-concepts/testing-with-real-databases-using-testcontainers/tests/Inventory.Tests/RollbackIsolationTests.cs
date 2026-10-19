using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// Isolation by transaction: every test runs inside a transaction that is never committed, so nothing it
// writes survives the test. Disposing the context without committing rolls the transaction back.
[TestClass]
[TestCategory("Integration")]
public sealed class RollbackIsolationTests(TestContext testContext) : IAsyncDisposable
{
    private static TestDatabase _database = null!;
    private InventoryDbContext _db = null!;

    // TODO-02: Create this class's database ("rollback_tests") once, in [ClassInitialize].
    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _)
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-02: create the database of this class.");
    }

    // TODO-02: Give every test a new context with a transaction already open (BeginTransactionAsync).
    [TestInitialize]
    public async Task BeginTransactionAsync()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-02: open a context and begin a transaction.");
    }

    // TODO-02: Dispose the context after the test. The transaction was never committed, so it rolls back.
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private CancellationToken Ct => testContext.CancellationToken;

    // TODO-03: Write the three tests below with a StockService over _db.
    [TestMethod]
    public async Task ReceivedStockIsStoredInTheDatabase()
    {
        // Register SKU-1, receive 5, then read it back and expect OnHand == 5.
        // Careful: a query on the same context returns the instance it already tracks, even if nothing was
        // saved. Call _db.ChangeTracker.Clear() before reading back.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task AReservationBeyondTheStockIsRejectedAndChangesNothing()
    {
        // Receive 2, try to reserve 3: expect InsufficientStockException, and OnHand still 2 in the database.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task TheLowStockReportListsOnlyProductsUnderTheThreshold()
    {
        // Two products, only one under the threshold. Assert the exact list, so rows leaked by another test
        // would turn this red.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }
}
