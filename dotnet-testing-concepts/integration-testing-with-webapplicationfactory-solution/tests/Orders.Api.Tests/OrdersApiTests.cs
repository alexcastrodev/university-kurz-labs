using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Orders.Api.Orders;

namespace Orders.Api.Tests;

// One host for the whole class, so the singleton OrderStore is shared by every test in it. The reset in
// [TestInitialize] keeps the tests independent, and [DoNotParallelize] stops two tests from resetting
// and filling the same store at the same time.
[TestClass]
[DoNotParallelize]
[TestCategory("Integration")]
public sealed class OrdersApiTests(TestContext testContext)
{
    private static OrdersApiFactory _factory = null!;

    private CancellationToken Ct => testContext.CancellationToken;

    [ClassInitialize]
    public static void StartApi(TestContext _) => _factory = new OrdersApiFactory();

    [ClassCleanup]
    public static async Task StopApi() => await _factory.DisposeAsync();

    [TestInitialize]
    public void ResetState()
    {
        _factory.Services.GetRequiredService<OrderStore>().Clear();
        _factory.Inventory.Clear();
    }

    private Task<HttpResponseMessage> PlaceAsync(string sku, int quantity) =>
        _factory.CreateClient().PostAsJsonAsync("/orders", new PlaceOrderRequest(sku, quantity), Ct);

    private async Task<Order> PlaceOrderAsync(string sku, int quantity)
    {
        var response = await PlaceAsync(sku, quantity);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<Order>(Ct))!;
    }

    [TestMethod]
    public async Task ShouldPlaceAnOrderAndPointToItWithTheLocationHeader()
    {
        var response = await PlaceAsync("KB-1", 2);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<Order>(Ct);
        Assert.AreEqual(new Order(order!.Id, "KB-1", 2), order);
        Assert.AreEqual($"/orders/{order.Id}", response.Headers.Location!.OriginalString);
    }

    [TestMethod]
    public async Task ShouldReturnAPlacedOrderById()
    {
        var placed = await PlaceOrderAsync("KB-1", 1);

        // GetFromJsonAsync is fine for a 200: it throws HttpRequestException on any other status.
        var order = await _factory.CreateClient().GetFromJsonAsync<Order>($"/orders/{placed.Id}", Ct);

        Assert.AreEqual(placed, order);
    }

    [TestMethod]
    public async Task ShouldReturnProblemDetailsForAnUnknownOrder()
    {
        var response = await _factory.CreateClient().GetAsync("/orders/999", Ct);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
        Assert.AreEqual(404, problem!.Status);
        Assert.AreEqual("Order not found", problem.Title);
    }

    [TestMethod]
    public async Task ShouldRejectAQuantityJustAboveTheConfiguredLimit()
    {
        // The factory sets the limit to 3 and appsettings.json says 10. Asking for 4 only fails if the
        // override really reached Program.cs; asking for 50 would fail with either limit.
        var response = await PlaceAsync("KB-1", OrdersApiFactory.MaxQuantity + 1);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(Ct);
        var error = Assert.ContainsSingle(problem!.Errors["quantity"]);
        Assert.AreEqual("The quantity must be between 1 and 3.", error);
    }

    [TestMethod]
    public async Task ShouldAcceptAQuantityExactlyAtTheConfiguredLimit()
    {
        var response = await PlaceAsync("KB-1", OrdersApiFactory.MaxQuantity);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturn409WhenTheInventoryHasNotEnoughStock()
    {
        _factory.Inventory.SetStock("KB-1", 1);

        var response = await PlaceAsync("KB-1", 2);

        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(Ct);
        Assert.AreEqual("Not enough stock", problem!.Title);
    }

    [TestMethod]
    public async Task ShouldListExactlyTheOrdersPlacedInThisTest()
    {
        await PlaceOrderAsync("KB-1", 1);
        await PlaceOrderAsync("MS-1", 2);

        var orders = await _factory.CreateClient().GetFromJsonAsync<List<Order>>("/orders", Ct);

        // Without the reset in [TestInitialize], orders from the other tests are still in the singleton.
        CollectionAssert.AreEqual(new[] { "KB-1", "MS-1" }, orders!.Select(o => o.Sku).ToArray());
    }

    [TestMethod]
    public async Task ShouldRedirectLatestToTheNewestOrder()
    {
        await PlaceOrderAsync("KB-1", 1);
        var newest = await PlaceOrderAsync("MS-1", 1);
        // The default client follows redirects and would report the 200 of the page it landed on.
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/orders/latest", Ct);

        Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
        Assert.AreEqual($"/orders/{newest.Id}", response.Headers.Location!.OriginalString);
    }
}
