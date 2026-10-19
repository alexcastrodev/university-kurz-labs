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

    // TODO-04: Write the tests below.
    [TestMethod]
    public async Task ATenantSeesOnlyItsOwnOrders()
    {
        // Place "A-1" for acme and "G-1" for globex. A context for acme lists only ["A-1"].
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ADeletedOrderIsHiddenByDefault()
    {
        // Place "A-1" and "A-2", delete "A-1" with OrderService, then read with a NEW context: only ["A-2"].
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ListingDeletedOrdersKeepsTheTenantFilter()
    {
        // Delete "A-1" (acme) AND "G-1" (globex). DeletedNumbersAsync for acme must return ["A-1"].
        // Without a deleted order in the other tenant, this test cannot tell IgnoreQueryFilters() from
        // IgnoreQueryFilters([SoftDeleteFilter]) and passes for the wrong reason.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task IgnoringEveryFilterCrossesTenants()
    {
        // Learning test: IgnoreQueryFilters() with no names returns ["A-1", "G-1"] from an acme context.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }
}
