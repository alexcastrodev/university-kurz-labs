namespace Orders.Api.Tests;

[TestClass]
[TestCategory("Integration")]
public sealed class OrdersApiTests(TestContext testContext)
{
    private CancellationToken Ct => testContext.CancellationToken;

    // TODO-02: Share one factory (one host) between the tests of this class, the MSTest way: keep it in a
    // private static OrdersApiFactory field, create it in a public static [ClassInitialize] method that
    // takes a TestContext, and await its DisposeAsync in a public static [ClassCleanup] method.
    // (xUnit would use IClassFixture<OrdersApiFactory>.)
    // OrderStore is a singleton, so every test of the class sees the same store. Add a [TestInitialize]
    // method that resets it (resolve it from _factory.Services) and resets _factory.Inventory.
    // MSTestSettings.cs runs test methods in parallel: what happens if two tests reset and fill the same
    // store at the same time? Look at [DoNotParallelize].

    // TODO-03: Write the happy path and the not found case. Send the production record
    // (new PlaceOrderRequest(sku, quantity)) with PostAsJsonAsync, so a typo in a property name cannot
    // turn into a 400 that your test mistakes for a validation error.
    [TestMethod]
    public async Task ShouldPlaceAnOrderAndPointToItWithTheLocationHeader()
    {
        // 201, the Order in the body (ReadFromJsonAsync<Order>), and Location is /orders/{id}.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturnAPlacedOrderById()
    {
        // GetFromJsonAsync<Order> is the shortest way to read a 200. What does it do on a 404?
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturnProblemDetailsForAnUnknownOrder()
    {
        // Status 404, content type application/problem+json, and ReadFromJsonAsync<ProblemDetails>
        // (Microsoft.AspNetCore.Mvc) with the title the endpoint sets.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    // TODO-04: The limit. The factory sets it to 3, appsettings.json says 10.
    [TestMethod]
    public async Task ShouldRejectAQuantityJustAboveTheConfiguredLimit()
    {
        // Pick the quantity carefully: which number fails with a limit of 3 but passes with a limit of 10?
        // Read the body as HttpValidationProblemDetails (Microsoft.AspNetCore.Http) and check the message
        // under "quantity".
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ShouldAcceptAQuantityExactlyAtTheConfiguredLimit()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    // TODO-05: The inventory is the fake from TODO-00. Give one SKU too little stock.
    [TestMethod]
    public async Task ShouldReturn409WhenTheInventoryHasNotEnoughStock()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    // TODO-06: Place two orders and check that GET /orders returns exactly those two, in order.
    // Then comment out your reset from TODO-02 and run the whole class. Why does this test fail now?
    [TestMethod]
    public async Task ShouldListExactlyTheOrdersPlacedInThisTest()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    // TODO-07: GET /orders/latest answers with a 302 to the newest order. The client from CreateClient()
    // follows redirects (WebApplicationFactoryClientOptions.AllowAutoRedirect is true by default), so a
    // test with it sees the 200 of the page it landed on. Would that test notice if the endpoint returned
    // the order directly, or redirected to the oldest one? Place two orders and assert the 302 and the
    // Location header.
    [TestMethod]
    public async Task ShouldRedirectLatestToTheNewestOrder()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }
}
