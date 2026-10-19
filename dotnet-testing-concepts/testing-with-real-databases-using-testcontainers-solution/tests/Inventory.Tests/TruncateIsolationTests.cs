using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// TODO-07 (optional): the same reset with plain SQL. TRUNCATE is what Respawn runs on PostgreSQL, but you
// name the tables yourself, and RESTART IDENTITY also resets the id sequences, which Respawn does not by default.
[TestClass]
[TestCategory("Integration")]
public sealed class TruncateIsolationTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("truncate_tests");

    [TestInitialize]
    public async Task TruncateAsync()
    {
        await using var db = _database.NewContext();
        await db.Database.ExecuteSqlRawAsync("""TRUNCATE "Products" RESTART IDENTITY CASCADE""", testContext.CancellationToken);
    }

    [TestMethod]
    [DataRow("SKU-1")]
    [DataRow("SKU-2")]
    public async Task IdsStartAgainAfterEveryReset(string sku)
    {
        await using var db = _database.NewContext();

        var product = await new StockService(db).RegisterAsync(sku, "Keyboard", testContext.CancellationToken);

        Assert.AreEqual(1, product.Id);
    }
}
