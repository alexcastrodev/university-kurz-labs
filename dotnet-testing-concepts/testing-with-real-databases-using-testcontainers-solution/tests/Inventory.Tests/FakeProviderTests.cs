using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// Learning tests: they pin down what the fakes do NOT check. Each one passes, and each one describes a bug
// that a suite built on the fake would ship. RespawnIsolationTests shows PostgreSQL rejecting the same data.
[TestClass]
public sealed class FakeProviderTests(TestContext testContext)
{
    [TestMethod]
    public async Task TheInMemoryProviderAcceptsADuplicateSku()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var db = new InventoryDbContext(options);
        var service = new StockService(db);

        await service.RegisterAsync("SKU-1", "Keyboard", testContext.CancellationToken);
        await service.RegisterAsync("SKU-1", "Another keyboard", testContext.CancellationToken);

        Assert.AreEqual(2, await db.Products.CountAsync(testContext.CancellationToken));
    }

    [TestMethod]
    public async Task SqliteAcceptsASkuLongerThanTheColumn()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync(testContext.CancellationToken);
        var options = new DbContextOptionsBuilder<InventoryDbContext>().UseSqlite(connection).Options;
        await using var db = new InventoryDbContext(options);
        await db.Database.EnsureCreatedAsync(testContext.CancellationToken);
        var tooLong = new string('X', Product.SkuMaxLength + 1);

        await new StockService(db).RegisterAsync(tooLong, "Keyboard", testContext.CancellationToken);

        db.ChangeTracker.Clear();
        Assert.AreEqual(tooLong, (await db.Products.SingleAsync(testContext.CancellationToken)).Sku);
    }
}
