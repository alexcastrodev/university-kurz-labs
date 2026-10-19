using System.Diagnostics;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Inventory.Tests;

// One PostgreSQL container for the whole test assembly. Starting a container takes seconds, so it is
// started once in [AssemblyInitialize] and every test class creates its own database inside it.
[TestClass]
public static class PostgresContainer
{
    private static PostgreSqlContainer? _container;

    [AssemblyInitialize]
    public static async Task StartAsync(TestContext testContext)
    {
        var stopwatch = Stopwatch.StartNew();
        _container = new PostgreSqlBuilder("postgres:18-alpine").Build();
        await _container.StartAsync(testContext.CancellationToken);
        testContext.WriteLine($"PostgreSQL container started in {stopwatch.ElapsedMilliseconds} ms");
    }

    [AssemblyCleanup]
    public static async Task StopAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    // Creates an empty database in the shared container and returns its connection string.
    public static async Task<string> CreateDatabaseAsync(string name)
    {
        var admin = Container.GetConnectionString();
        await using (var connection = new NpgsqlConnection(admin))
        {
            await connection.OpenAsync();
            await using var command = new NpgsqlCommand($"CREATE DATABASE \"{name}\"", connection);
            await command.ExecuteNonQueryAsync();
        }

        return new NpgsqlConnectionStringBuilder(admin) { Database = name }.ConnectionString;
    }

    private static PostgreSqlContainer Container =>
        _container ?? throw new InvalidOperationException("The container is started in [AssemblyInitialize].");
}
