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

    // TODO-03: Write the tests below.
    [TestMethod]
    public async Task SearchByCustomerIgnoresCase()
    {
        // Place "A-1" for "ACME Corp" and "A-2" for "Initech" (tenant acme). SearchByCustomerAsync("acme") must
        // return exactly ["A-1"]. PostgreSQL compares strings case-sensitively, so this proves the query uses ILIKE.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task SummariesAreComputedFromTheLines()
    {
        // "A-1" with lines (KB, 2 x 10.50) and (MS, 1 x 4.00), "A-2" with no lines. Expect the summaries
        // (A-1, 2 lines, 25.00) and (A-2, 0 lines, 0). The order with no lines checks the SQL SUM over nothing.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task AQueryWithAComparisonTypeDoesNotTranslate()
    {
        // Learning test: Where(o => o.CustomerName.Equals("acme corp", StringComparison.OrdinalIgnoreCase))
        // throws InvalidOperationException ("could not be translated") on PostgreSQL. The InMemory provider runs it.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }
}
