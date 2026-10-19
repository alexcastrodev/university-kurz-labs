using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ordering.Tests;

// A database that belongs to one test class, with the schema built by the real migrations, the same way
// production gets it. Tests share it and call ResetAsync before each test.
internal sealed class TestDatabase
{
    private TestDatabase(string connectionString) => ConnectionString = connectionString;

    public string ConnectionString { get; }

    public static async Task<TestDatabase> CreateAsync(string name)
    {
        var database = new TestDatabase(await PostgresContainer.CreateDatabaseAsync(name));
        await using var db = database.NewContext("migrations");
        await db.Database.MigrateAsync();
        return database;
    }

    public OrdersDbContext NewContext(string tenantId, params IInterceptor[] interceptors) =>
        new(new DbContextOptionsBuilder<OrdersDbContext>()
            .UseNpgsql(ConnectionString)
            .AddInterceptors(interceptors)
            .Options, tenantId);

    public async Task ResetAsync(CancellationToken cancellationToken)
    {
        await using var db = NewContext("reset");
        await db.Database.ExecuteSqlRawAsync(
            """TRUNCATE "OrderLine", "Orders" RESTART IDENTITY CASCADE""", cancellationToken);
    }
}
