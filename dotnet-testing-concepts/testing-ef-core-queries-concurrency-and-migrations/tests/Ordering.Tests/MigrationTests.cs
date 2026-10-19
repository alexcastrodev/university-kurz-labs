using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class MigrationTests(TestContext testContext)
{
    // TODO-07: Write the tests below.
    [TestMethod]
    public async Task EveryMigrationAppliesToAnEmptyDatabase()
    {
        // Create a database WITHOUT TestDatabase (PostgresContainer.CreateDatabaseAsync), run MigrateAsync, and
        // compare GetMigrations() with GetAppliedMigrationsAsync(). Then place an order on that schema.
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }

    [TestMethod]
    public void TheModelHasNoChangesMissingAMigration()
    {
        // Assert.IsFalse(db.Database.HasPendingModelChanges()). It needs a context configured with UseNpgsql,
        // but it never opens the connection.
        Assert.Fail("TODO-07: write this test.");
    }
}
