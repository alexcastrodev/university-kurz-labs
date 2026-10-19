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

    // TODO-08: Write the tests below.
    [TestMethod]
    [DataRow(1)]
    [DataRow(5)]
    public async Task SummariesTakeOneCommandWhateverTheNumberOfOrders(int orders)
    {
        // Place `orders` orders with one line each. Create the context with a CommandCounter, call SummariesAsync,
        // and assert both the number of summaries and that exactly 1 command ran. A functional assertion alone
        // stays green when the code runs one query per order.
        await Task.CompletedTask;
        Assert.Fail("TODO-08: write this test.");
    }
}
