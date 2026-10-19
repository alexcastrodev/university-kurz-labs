using System.Net.Http.Json;
using Orders.Api.Orders;

namespace Orders.Api.Tests;

// .NET 10: the same factory can start the app on a real Kestrel socket instead of the in-memory TestServer.
[TestClass]
[TestCategory("Integration")]
public sealed class KestrelTests(TestContext testContext)
{
    [TestMethod]
    public async Task ShouldServeTheApiOverARealSocket()
    {
        await using var factory = new OrdersApiFactory();
        factory.UseKestrel(0); // port 0: the OS picks a free port. Must be called before the host starts.
        factory.StartServer();

        var client = factory.CreateClient();
        var orders = await client.GetFromJsonAsync<List<Order>>("/orders", testContext.CancellationToken);

        Assert.AreEqual("127.0.0.1", client.BaseAddress!.Host);
        Assert.AreNotEqual(80, client.BaseAddress.Port);
        Assert.IsEmpty(orders!);
        Assert.ThrowsExactly<NotSupportedException>(() => factory.Server);
    }
}
