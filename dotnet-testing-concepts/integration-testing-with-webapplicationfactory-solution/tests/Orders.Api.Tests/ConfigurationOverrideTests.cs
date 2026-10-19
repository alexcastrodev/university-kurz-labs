using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Orders.Api.Orders;

namespace Orders.Api.Tests;

// A learning test: it pins down how the framework behaves, so the reason UseSetting is used is written down.
[TestClass]
[TestCategory("Integration")]
public sealed class ConfigurationOverrideTests(TestContext testContext)
{
    [TestMethod]
    public async Task ConfigureAppConfigurationArrivesTooLateForAValueReadBeforeBuild()
    {
        await using var factory = new OrdersApiFactory().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Orders:MaxQuantity"] = "1" })));

        // With a limit of 1 this would be a 400. Program.cs read the value before Build, so the factory's
        // UseSetting (3) is what OrderRules received, and 2 is accepted.
        var response = await factory.CreateClient()
            .PostAsJsonAsync("/orders", new PlaceOrderRequest("KB-1", 2), testContext.CancellationToken);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [TestMethod]
    public async Task ConfigureAppConfigurationIsVisibleToIConfigurationAfterBuild()
    {
        await using var factory = new OrdersApiFactory().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Orders:MaxQuantity"] = "1" })));

        var configuration = (IConfiguration)factory.Services.GetService(typeof(IConfiguration))!;

        Assert.AreEqual("1", configuration["Orders:MaxQuantity"]);
    }
}
