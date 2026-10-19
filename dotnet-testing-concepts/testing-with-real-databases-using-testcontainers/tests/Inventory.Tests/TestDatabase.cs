using Microsoft.EntityFrameworkCore;

namespace Inventory.Tests;

// A database that belongs to one test class. The schema comes from the model with EnsureCreated, because
// this lab has no migrations (the next lab applies real migrations instead).
internal sealed class TestDatabase
{
    private TestDatabase(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get; }

    // TODO-01: Create the database with PostgresContainer.CreateDatabaseAsync, then create the schema with
    // EnsureCreatedAsync on a context that you dispose right away.
    public static async Task<TestDatabase> CreateAsync(string name)
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-01: create the database and its schema.");
        return null!;
    }

    public InventoryDbContext NewContext() =>
        new(new DbContextOptionsBuilder<InventoryDbContext>().UseNpgsql(ConnectionString).Options);
}
