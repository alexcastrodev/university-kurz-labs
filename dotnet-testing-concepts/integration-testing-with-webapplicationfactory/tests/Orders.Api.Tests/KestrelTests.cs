namespace Orders.Api.Tests;

// TODO-09 (optional): .NET 10 lets the factory start a real Kestrel server instead of the in-memory TestServer.
[TestClass]
[TestCategory("Integration")]
public sealed class KestrelTests(TestContext testContext)
{
    [TestMethod]
    public async Task ShouldServeTheApiOverARealSocket()
    {
        // factory.UseKestrel(0) then factory.StartServer(), before anything else touches the factory.
        // Check the client's BaseAddress (127.0.0.1 and a real port), call GET /orders, and see what
        // factory.Server does now.
        _ = testContext;
        await Task.CompletedTask;
        Assert.Fail("TODO-09 (optional): write this test.");
    }
}
