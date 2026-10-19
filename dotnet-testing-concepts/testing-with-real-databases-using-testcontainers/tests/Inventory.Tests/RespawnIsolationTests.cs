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

    // TODO-04: Create this class's database ("respawn_tests"), open an NpgsqlConnection to it, and build
    // the Respawner once with DbAdapter.Postgres and SchemasToInclude = ["public"].
    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _)
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: create the database and the Respawner.");
    }

    // TODO-04: Reset the database before every test. Respawn needs an open connection for PostgreSQL.
    [TestInitialize]
    public async Task ResetAsync()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: reset the database.");
    }

    private CancellationToken Ct => testContext.CancellationToken;

    // TODO-05: Write the four tests below. Write with one context and assert with a new one
    // (_database.NewContext()), so the assertion reads what was committed.
    [TestMethod]
    public async Task EveryTestStartsWithAnEmptyCatalog()
    {
        // Register one product and expect exactly 1 product. Run the whole class: without the reset, this
        // test passes or fails depending on which tests ran before it.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ReceivedStockIsCommitted()
    {
        // Register and receive 5 in one context, read OnHand back in another.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ASecondProductWithTheSameSkuIsRejected()
    {
        // Expect DbUpdateException whose InnerException is a PostgresException with
        // SqlState == PostgresErrorCodes.UniqueViolation, and still 1 product afterwards.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ASkuLongerThanTheColumnIsRejected()
    {
        // A SKU of Product.SkuMaxLength + 1 characters: PostgreSQL answers with
        // PostgresErrorCodes.StringDataRightTruncation.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }
}
