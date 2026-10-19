using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// A database that belongs to one test class. The schema comes from the model with EnsureCreated, because
// this lab has no migrations (the next lab applies real migrations instead).
internal sealed class TestDatabase
{
    private TestDatabase(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get; }

    public static async Task<TestDatabase> CreateAsync(string name)
    {
        var database = new TestDatabase(await PostgresContainer.CreateDatabaseAsync(name));
        await using var db = database.NewContext();
        await db.Database.EnsureCreatedAsync();
        return database;
    }

    public InventoryDbContext NewContext() =>
        new(new DbContextOptionsBuilder<InventoryDbContext>().UseNpgsql(ConnectionString).Options);
}
