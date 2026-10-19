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

    // TODO-05: Write the tests below.
    [TestMethod]
    public async Task TheSecondOfTwoConcurrentWritersGetsAConflict()
    {
        // Two contexts load the same order. The first changes the customer and saves; the second changes it
        // and saves: DbUpdateConcurrencyException. A third context sees the first writer's value.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task OneContextCannotConflictWithItself()
    {
        // Learning test: ONE context loads, changes, saves, changes, saves. No exception. This is the
        // "concurrency test" that proves nothing: it stays green even without a concurrency token.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task EveryUpdateMovesTheVersion()
    {
        // Place an order, keep its Version, update it in another context, and read the Version again: it changed.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }
}
