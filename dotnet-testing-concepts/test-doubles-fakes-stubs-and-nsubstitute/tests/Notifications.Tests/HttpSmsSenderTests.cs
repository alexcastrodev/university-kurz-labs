namespace Notifications.Tests;

// Optional. These tests are ignored until you remove the [Ignore] attribute from each one.
[TestClass]
public sealed class HttpSmsSenderTests(TestContext testContext)
{
    [TestMethod]
    [Ignore("TODO-08 (optional)")]
    public async Task TheGatewayAcceptingTheMessageMeansDelivered()
    {
        // Build an HttpClient over a StubHttpHandler (BaseAddress https://sms.example.com/) answering 202.
        // Check the result, the method, the URL and the body. Compare the body after deserializing it into
        // HttpSmsSender.SmsRequest: comparing the raw JSON string fails for a reason you will discover.
        await Task.Delay(0, testContext.CancellationToken);
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    [Ignore("TODO-08 (optional)")]
    public async Task TheGatewayRefusingTheNumberMeansNotDelivered()
    {
        await Task.Delay(0, testContext.CancellationToken);
        Assert.Fail("TODO-08 (optional): write this test.");
    }

    [TestMethod]
    [Ignore("TODO-09 (optional)")]
    public void NSubstituteCannotReplaceASealedClass()
    {
        // A learning test: what happens when you call Substitute.For<HttpSmsSender>(new HttpClient())?
        // Pin the exact exception type down with Assert.ThrowsExactly.
        Assert.Fail("TODO-09 (optional): write this test.");
    }
}
