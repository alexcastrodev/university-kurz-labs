using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

// These queries run as SQL in PostgreSQL. A fake provider runs them as C# instead, so it cannot tell you
// whether they translate, or what the database's own string rules return.
[TestClass]
[TestCategory("Integration")]
public sealed class QueryTranslationTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("query_tests");

    [TestInitialize]
    public Task ResetAsync() => _database.ResetAsync(testContext.CancellationToken);

    [TestMethod]
    public async Task SearchByCustomerIgnoresCase()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1", "ACME Corp");
        await _database.PlaceAsync(Seed.Acme, "A-2", "Initech");

        await using var db = _database.NewContext(Seed.Acme);
        var found = await new OrderQueries(db).SearchByCustomerAsync("acme", testContext.CancellationToken);

        CollectionAssert.AreEqual(new[] { "A-1" }, found);
    }

    [TestMethod]
    public async Task SummariesAreComputedFromTheLines()
    {
        await _database.PlaceAsync(Seed.Acme, "A-1", "ACME Corp", new NewLine("KB", 2, 10.50m), new NewLine("MS", 1, 4.00m));
        await _database.PlaceAsync(Seed.Acme, "A-2", "ACME Corp");

        await using var db = _database.NewContext(Seed.Acme);
        var summaries = await new OrderQueries(db).SummariesAsync(testContext.CancellationToken);

        CollectionAssert.AreEqual(
            new[] { new OrderSummary("A-1", "ACME Corp", 2, 25.00m), new OrderSummary("A-2", "ACME Corp", 0, 0m) },
            summaries);
    }

    [TestMethod]
    public async Task AQueryWithAComparisonTypeDoesNotTranslate()
    {
        await using var db = _database.NewContext(Seed.Acme);

        // The InMemory provider runs this happily; a relational provider refuses it.
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => db.Orders
            .Where(o => o.CustomerName.Equals("acme corp", StringComparison.OrdinalIgnoreCase))
            .ToListAsync(testContext.CancellationToken));
    }
}
