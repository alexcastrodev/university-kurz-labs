using Microsoft.EntityFrameworkCore;

namespace Ordering.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class MigrationTests(TestContext testContext)
{
    [TestMethod]
    public async Task EveryMigrationAppliesToAnEmptyDatabase()
    {
        var connectionString = await PostgresContainer.CreateDatabaseAsync("empty_for_migrations");
        var options = new DbContextOptionsBuilder<OrdersDbContext>().UseNpgsql(connectionString).Options;

        await using (var db = new OrdersDbContext(options, Seed.Acme))
        {
            await db.Database.MigrateAsync(testContext.CancellationToken);

            CollectionAssert.AreEqual(
                db.Database.GetMigrations().ToList(),
                (await db.Database.GetAppliedMigrationsAsync(testContext.CancellationToken)).ToList());
        }

        // The migrated schema accepts what the model writes.
        await using var check = new OrdersDbContext(options, Seed.Acme);
        await new OrderService(check).PlaceAsync("A-1", "ACME Corp", [new NewLine("KB", 1, 9.99m)], testContext.CancellationToken);
    }

    [TestMethod]
    public void TheModelHasNoChangesMissingAMigration()
    {
        // No connection is opened: the model is compared with the last migration's snapshot.
        var options = new DbContextOptionsBuilder<OrdersDbContext>().UseNpgsql("Host=unused").Options;
        using var db = new OrdersDbContext(options, Seed.Acme);

        Assert.IsFalse(db.Database.HasPendingModelChanges(), "Run `dotnet ef migrations add` for the model changes.");
    }
}
