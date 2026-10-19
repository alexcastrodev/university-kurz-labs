using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class QueryFilterTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("filter_tests");

    [TestInitialize]
    public Task ResetAsync() => _database.ResetAsync(testContext.CancellationToken);

    private CancellationToken Ct => testContext.CancellationToken;

    [TestMethod]
    public async Task ATenantSeesOnlyItsOwnOrders()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");
        await _database.PlaceAsync(Seed.Globex, "G-1");

        await using var db = _database.NewContext(Seed.Acme);
        var numbers = await db.Orders.Select(o => o.Number).ToListAsync(Ct);

        CollectionAssert.AreEqual(new[] { "A-1" }, numbers);
    }

    [TestMethod]
    public async Task ADeletedOrderIsHiddenByDefault()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");
        await _database.PlaceAsync(Seed.Acme, "A-2");
        await using (var db = _database.NewContext(Seed.Acme))
        {
            await new OrderService(db).DeleteAsync("A-1", Ct);
        }

        await using var check = _database.NewContext(Seed.Acme);
        CollectionAssert.AreEqual(new[] { "A-2" }, await check.Orders.Select(o => o.Number).ToListAsync(Ct));
    }

    [TestMethod]
    public async Task ListingDeletedOrdersKeepsTheTenantFilter()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");
        await _database.PlaceAsync(Seed.Globex, "G-1");
        await using (var acme = _database.NewContext(Seed.Acme))
        {
            await new OrderService(acme).DeleteAsync("A-1", Ct);
        }
        await using (var globex = _database.NewContext(Seed.Globex))
        {
            await new OrderService(globex).DeleteAsync("G-1", Ct);
        }

        await using var check = _database.NewContext(Seed.Acme);
        var deleted = await new OrderQueries(check).DeletedNumbersAsync(Ct);

        // Without a deleted order in another tenant, this test could not tell the two IgnoreQueryFilters apart.
        CollectionAssert.AreEqual(new[] { "A-1" }, deleted);
    }

    [TestMethod]
    public async Task IgnoringEveryFilterCrossesTenants()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");
        await _database.PlaceAsync(Seed.Globex, "G-1");

        await using var db = _database.NewContext(Seed.Acme);
        var all = await db.Orders.IgnoreQueryFilters().OrderBy(o => o.Number).Select(o => o.Number).ToListAsync(Ct);

        CollectionAssert.AreEqual(new[] { "A-1", "G-1" }, all);
    }
}
