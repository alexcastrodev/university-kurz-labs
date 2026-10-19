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

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("rollback_tests");

    [TestInitialize]
    public async Task BeginTransactionAsync()
    {
        _db = _database.NewContext();
        await _db.Database.BeginTransactionAsync(testContext.CancellationToken);
    }

    public ValueTask DisposeAsync() => _db.DisposeAsync();

    private CancellationToken Ct => testContext.CancellationToken;

    [TestMethod]
    public async Task ReceivedStockIsStoredInTheDatabase()
    {
        var service = new StockService(_db);
        await service.RegisterAsync("SKU-1", "Keyboard", Ct);

        await service.ReceiveAsync("SKU-1", 5, Ct);

        // Without Clear, the query below returns the tracked instance and never looks at the row.
        _db.ChangeTracker.Clear();
        var stored = await _db.Products.SingleAsync(p => p.Sku == "SKU-1", Ct);
        Assert.AreEqual(5, stored.OnHand);
    }

    [TestMethod]
    public async Task AReservationBeyondTheStockIsRejectedAndChangesNothing()
    {
        var service = new StockService(_db);
        await service.RegisterAsync("SKU-1", "Keyboard", Ct);
        await service.ReceiveAsync("SKU-1", 2, Ct);

        await Assert.ThrowsExactlyAsync<InsufficientStockException>(() => service.ReserveAsync("SKU-1", 3, Ct));

        _db.ChangeTracker.Clear();
        Assert.AreEqual(2, (await _db.Products.SingleAsync(p => p.Sku == "SKU-1", Ct)).OnHand);
    }

    [TestMethod]
    public async Task TheLowStockReportListsOnlyProductsUnderTheThreshold()
    {
        var service = new StockService(_db);
        await service.RegisterAsync("SKU-1", "Keyboard", Ct);
        await service.RegisterAsync("SKU-2", "Mouse", Ct);
        await service.ReceiveAsync("SKU-2", 10, Ct);

        var low = await service.LowStockSkusAsync(threshold: 3, Ct);

        // An exact list: rows leaked by another test would show up here.
        CollectionAssert.AreEqual(new[] { "SKU-1" }, low);
    }
}
