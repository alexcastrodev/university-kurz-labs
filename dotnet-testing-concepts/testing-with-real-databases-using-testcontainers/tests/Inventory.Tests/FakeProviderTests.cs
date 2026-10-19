using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// Learning tests: they pin down what the fakes do NOT check. Each one passes, and each one describes a bug
// that a suite built on the fake would ship. RespawnIsolationTests shows PostgreSQL rejecting the same data.
[TestClass]
public sealed class FakeProviderTests(TestContext testContext)
{
    // TODO-06: Write the two tests below.
    [TestMethod]
    public async Task TheInMemoryProviderAcceptsADuplicateSku()
    {
        // UseInMemoryDatabase with a unique name. Register SKU-1 twice: no exception, and 2 products.
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    [TestMethod]
    public async Task SqliteAcceptsASkuLongerThanTheColumn()
    {
        // SQLite in memory: open a SqliteConnection("DataSource=:memory:"), UseSqlite(connection),
        // EnsureCreated, then register a SKU of Product.SkuMaxLength + 1 characters. It is stored whole.
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }
}
