namespace Orders.Api.Tests;

// TODO-08 (optional): A learning test, so the reason for UseSetting is written down.
[TestClass]
[TestCategory("Integration")]
public sealed class ConfigurationOverrideTests(TestContext testContext)
{
    [TestMethod]
    public async Task ConfigureAppConfigurationArrivesTooLateForAValueReadBeforeBuild()
    {
        // new OrdersApiFactory().WithWebHostBuilder(b => b.ConfigureAppConfiguration(...)) with
        // Orders:MaxQuantity = 1 in an in-memory collection. Is a quantity of 2 accepted or rejected?
        _ = testContext;
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    public async Task ConfigureAppConfigurationIsVisibleToIConfigurationAfterBuild()
    {
        // Same override, but read IConfiguration from factory.Services. What does it say?
        await Task.CompletedTask;
        Assert.Fail("TODO-08 (optional): write this test.");
    }
}
