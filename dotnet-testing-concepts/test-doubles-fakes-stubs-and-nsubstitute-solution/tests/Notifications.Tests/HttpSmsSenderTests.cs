using System.Net;
using System.Text.Json;

namespace Notifications.Tests;

// The adapter is tested with a hand-written HttpMessageHandler stub, not with NSubstitute.
[TestClass]
public sealed class HttpSmsSenderTests(TestContext testContext)
{
    private static HttpClient ClientWith(StubHttpHandler handler) =>
        new(handler) { BaseAddress = new Uri("https://sms.example.com/") };

    [TestMethod]
    public async Task TheGatewayAcceptingTheMessageMeansDelivered()
    {
        var handler = new StubHttpHandler(HttpStatusCode.Accepted);

        var delivered = await new HttpSmsSender(ClientWith(handler)).SendAsync("+15550100", "hello", testContext.CancellationToken);

        Assert.IsTrue(delivered);
        Assert.AreEqual(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.AreEqual("https://sms.example.com/messages", handler.LastRequest.RequestUri!.ToString());
        // Compare what the JSON means, not its bytes: System.Text.Json escapes '+' as \u002B by default.
        var sent = JsonSerializer.Deserialize<HttpSmsSender.SmsRequest>(handler.LastBody!, JsonSerializerOptions.Web);
        Assert.AreEqual(new HttpSmsSender.SmsRequest("+15550100", "hello"), sent);
    }

    [TestMethod]
    public async Task TheGatewayRefusingTheNumberMeansNotDelivered()
    {
        var handler = new StubHttpHandler(HttpStatusCode.UnprocessableEntity);

        var delivered = await new HttpSmsSender(ClientWith(handler)).SendAsync("+0", "hello", testContext.CancellationToken);

        Assert.IsFalse(delivered);
    }

    [TestMethod]
    public void NSubstituteCannotReplaceASealedClass()
    {
        // A learning test: it pins down a fact about the library, not about our code. Castle DynamicProxy
        // cannot derive from a sealed class, so the service depends on ISmsSender instead.
        Assert.ThrowsExactly<TypeLoadException>(() => Substitute.For<HttpSmsSender>(new HttpClient()));
    }
}
