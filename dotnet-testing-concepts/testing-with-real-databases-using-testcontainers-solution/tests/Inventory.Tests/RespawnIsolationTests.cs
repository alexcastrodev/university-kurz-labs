using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;

namespace Inventory.Tests;

// Isolation by reset: the code under test commits for real, and Respawn empties the tables before each
// test. Use it when the test cannot hand its transaction to the code, for example code that opens its own
// context or its own transaction, and when you want to assert on what was really committed.
[TestClass]
[TestCategory("Integration")]
public sealed class RespawnIsolationTests(TestContext testContext)
{
    private static TestDatabase _database = null!;
    private static Respawner _respawner = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _)
    {
        _database = await TestDatabase.CreateAsync("respawn_tests");
        await using var connection = new NpgsqlConnection(_database.ConnectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
        });
    }

    [TestInitialize]
    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(_database.ConnectionString);
        await connection.OpenAsync(testContext.CancellationToken);
        await _respawner.ResetAsync(connection);
    }

    private CancellationToken Ct => testContext.CancellationToken;

    [TestMethod]
    public async Task EveryTestStartsWithAnEmptyCatalog()
    {
        await using (var db = _database.NewContext())
        {
            await new StockService(db).RegisterAsync("SKU-1", "Keyboard", Ct);
        }

        await using var check = _database.NewContext();
        Assert.AreEqual(1, await check.Products.CountAsync(Ct));
    }

    [TestMethod]
    public async Task ReceivedStockIsCommitted()
    {
        await using (var db = _database.NewContext())
        {
            var service = new StockService(db);
            await service.RegisterAsync("SKU-1", "Keyboard", Ct);
            await service.ReceiveAsync("SKU-1", 5, Ct);
        }

        // A new context reads the committed row, not an instance the writer still tracks.
        await using var check = _database.NewContext();
        Assert.AreEqual(5, (await check.Products.SingleAsync(Ct)).OnHand);
    }

    [TestMethod]
    public async Task ASecondProductWithTheSameSkuIsRejected()
    {
        await using (var db = _database.NewContext())
        {
            await new StockService(db).RegisterAsync("SKU-1", "Keyboard", Ct);
        }

        await using (var db = _database.NewContext())
        {
            var duplicate = await Assert.ThrowsExactlyAsync<DbUpdateException>(
                () => new StockService(db).RegisterAsync("SKU-1", "Another keyboard", Ct));
            var postgres = Assert.IsInstanceOfType<PostgresException>(duplicate.InnerException);
            Assert.AreEqual(PostgresErrorCodes.UniqueViolation, postgres.SqlState);
        }

        await using var check = _database.NewContext();
        Assert.AreEqual(1, await check.Products.CountAsync(Ct));
    }

    [TestMethod]
    public async Task ASkuLongerThanTheColumnIsRejected()
    {
        await using var db = _database.NewContext();

        var tooLong = await Assert.ThrowsExactlyAsync<DbUpdateException>(
            () => new StockService(db).RegisterAsync(new string('X', Product.SkuMaxLength + 1), "Keyboard", Ct));

        var postgres = Assert.IsInstanceOfType<PostgresException>(tooLong.InnerException);
        Assert.AreEqual(PostgresErrorCodes.StringDataRightTruncation, postgres.SqlState);
    }
}
