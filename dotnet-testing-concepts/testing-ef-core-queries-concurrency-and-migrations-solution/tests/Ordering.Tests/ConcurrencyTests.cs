using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class ConcurrencyTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("concurrency_tests");

    [TestInitialize]
    public Task ResetAsync() => _database.ResetAsync(testContext.CancellationToken);

    private CancellationToken Ct => testContext.CancellationToken;

    [TestMethod]
    public async Task TheSecondOfTwoConcurrentWritersGetsAConflict()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1", "ACME Corp");

        // Two contexts are two users: both read the same version of the row.
        await using var first = _database.NewContext(Seed.Acme);
        await using var second = _database.NewContext(Seed.Acme);
        var firstCopy = await first.Orders.SingleAsync(Ct);
        var secondCopy = await second.Orders.SingleAsync(Ct);

        firstCopy.ChangeCustomer("ACME Ltd");
        await first.SaveChangesAsync(Ct);
        secondCopy.ChangeCustomer("ACME Inc");

        await Assert.ThrowsExactlyAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync(Ct));
        await using var check = _database.NewContext(Seed.Acme);
        Assert.AreEqual("ACME Ltd", (await check.Orders.SingleAsync(Ct)).CustomerName);
    }

    [TestMethod]
    public async Task OneContextCannotConflictWithItself()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1", "ACME Corp");
        await using var db = _database.NewContext(Seed.Acme);
        var order = await db.Orders.SingleAsync(Ct);

        // After each save the context refreshes the tracked version, so the second save is never stale.
        // A "concurrency test" written this way stays green even without a concurrency token.
        order.ChangeCustomer("ACME Ltd");
        await db.SaveChangesAsync(Ct);
        order.ChangeCustomer("ACME Inc");
        await db.SaveChangesAsync(Ct);

        await using var check = _database.NewContext(Seed.Acme);
        Assert.AreEqual("ACME Inc", (await check.Orders.SingleAsync(Ct)).CustomerName);
    }

    [TestMethod]
    public async Task EveryUpdateMovesTheVersion()
    {
        var placed = await _database.PlaceAsync(Seed.Acme, "A-1", "ACME Corp");

        await using (var db = _database.NewContext(Seed.Acme))
        {
            (await db.Orders.SingleAsync(Ct)).ChangeCustomer("ACME Ltd");
            await db.SaveChangesAsync(Ct);
        }

        await using var check = _database.NewContext(Seed.Acme);
        Assert.AreNotEqual(placed.Version, (await check.Orders.SingleAsync(Ct)).Version);
    }
}
