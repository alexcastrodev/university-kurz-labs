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

    // TODO-00: Build a PostgreSqlContainer for the "postgres:18-alpine" image, start it with the token of
    // testContext, and keep it in _container. Write the startup time with testContext.WriteLine.
    [AssemblyInitialize]
    public static async Task StartAsync(TestContext testContext)
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-00: start the PostgreSQL container.");
    }

    // TODO-00: Dispose the container, so the run leaves nothing behind (Ryuk is only the safety net).
    [AssemblyCleanup]
    public static async Task StopAsync()
    {
        await Task.CompletedTask;
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
