using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class UniqueViolationTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("unique_tests");

    [TestInitialize]
    public Task ResetAsync() => _database.ResetAsync(testContext.CancellationToken);

    private CancellationToken Ct => testContext.CancellationToken;

    [TestMethod]
    public async Task ATakenNumberIsReportedAsADomainError()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");

        await using (var db = _database.NewContext(Seed.Acme))
        {
            var error = await Assert.ThrowsExactlyAsync<DuplicateOrderNumberException>(
                () => new OrderService(db).PlaceAsync("A-1", "Someone else", [], Ct));
            Assert.AreEqual("A-1", error.Number);
        }

        await using var check = _database.NewContext(Seed.Acme);
        Assert.AreEqual(1, await check.Orders.CountAsync(Ct));
    }

    [TestMethod]
    public async Task TheSameNumberIsFreeInAnotherTenant()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1");

        await _database.PlaceAsync(Seed.Globex, "A-1");

        await using var check = _database.NewContext(Seed.Acme);
        Assert.AreEqual(2, await check.Orders.IgnoreQueryFilters().CountAsync(o => o.Number == "A-1", Ct));
    }
}
