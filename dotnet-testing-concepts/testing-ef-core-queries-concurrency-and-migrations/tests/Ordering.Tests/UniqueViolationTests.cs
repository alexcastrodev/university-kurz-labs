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

    // TODO-06: Write the tests below.
    [TestMethod]
    public async Task ATakenNumberIsReportedAsADomainError()
    {
        // Place "A-1" twice for acme: the second throws DuplicateOrderNumberException (not DbUpdateException),
        // with Number == "A-1", and a new context still counts 1 order.
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    [TestMethod]
    public async Task TheSameNumberIsFreeInAnotherTenant()
    {
        // "A-1" for acme and "A-1" for globex both succeed. Count them with IgnoreQueryFilters().
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }
}
