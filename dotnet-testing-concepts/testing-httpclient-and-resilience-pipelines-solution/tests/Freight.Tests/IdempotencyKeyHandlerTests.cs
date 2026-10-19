using System.Net;

namespace Freight.Tests;

// A DelegatingHandler on its own: give it an inner handler and call it through an HttpMessageInvoker.
// No factory, no pipeline.
[TestClass]
public sealed class IdempotencyKeyHandlerTests(TestContext testContext)
{
    [TestMethod]
    public async Task APostGetsAFreshKeyAndAGetGetsNone()
    {
        var api = new FakeFreightApi().Respond(HttpStatusCode.OK).Respond(HttpStatusCode.OK).Respond(HttpStatusCode.OK);
        using var invoker = new HttpMessageInvoker(new IdempotencyKeyHandler { InnerHandler = api });

        await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://freight.test/quotes"), testContext.CancellationToken);
        await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Post, "http://freight.test/quotes"), testContext.CancellationToken);
        await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "http://freight.test/quotes"), testContext.CancellationToken);

        var (first, second, get) = (api.Sent[0], api.Sent[1], api.Sent[2]);
        Assert.IsNotNull(first.IdempotencyKey);
        Assert.AreNotEqual(first.IdempotencyKey, second.IdempotencyKey);
        Assert.IsNull(get.IdempotencyKey);
    }
}
