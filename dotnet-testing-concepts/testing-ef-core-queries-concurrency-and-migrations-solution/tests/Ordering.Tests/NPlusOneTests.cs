namespace Ordering.Tests;

// A functional test checks the numbers; it is just as green when the code runs one query per order.
// Counting the commands is what turns an N+1 into a failing test.
[TestClass]
[TestCategory("Integration")]
public sealed class NPlusOneTests(TestContext testContext)
{
    private static TestDatabase _database = null!;

    [ClassInitialize]
    public static async Task CreateDatabaseAsync(TestContext _) =>
        _database = await TestDatabase.CreateAsync("n_plus_one_tests");

    [TestInitialize]
    public Task ResetAsync() => _database.ResetAsync(testContext.CancellationToken);

    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    public async Task SummariesTakeOneCommandWhateverTheNumberOfOrders(int orders)
    {
        for (var i = 1; i <= orders; i++)
        {
            await _database.PlaceAsync(Seed.Acme, $"A-{i}", "ACME Corp", new NewLine("KB", i, 1m));
        }

        var counter = new CommandCounter();
        await using var db = _database.NewContext(Seed.Acme, counter);
        var summaries = await new OrderQueries(db).SummariesAsync(testContext.CancellationToken);

        Assert.HasCount(orders, summaries);
        Assert.HasCount(1, counter.Commands, string.Join(Environment.NewLine, counter.Commands));
    }
}
