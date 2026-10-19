namespace Freight.Tests;

// A DelegatingHandler on its own: give it an inner handler and call it through an HttpMessageInvoker.
// No factory, no pipeline.
[TestClass]
public sealed class IdempotencyKeyHandlerTests(TestContext testContext)
{
    [TestMethod]
    public async Task APostGetsAFreshKeyAndAGetGetsNone()
    {
        // TODO-01: new HttpMessageInvoker(new IdempotencyKeyHandler { InnerHandler = api }). Send two POSTs
        // and one GET (pass testContext.CancellationToken). Two different keys on the POSTs, none on the GET.
        await Task.CompletedTask;
        Assert.Fail($"TODO-01: write {testContext.TestName}.");
    }
}
