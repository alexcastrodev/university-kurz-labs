namespace Notes.Api.Tests;

// Happy-path tests only visit the endpoints someone remembered. This test visits every endpoint the app mapped.
[TestClass]
[TestCategory("Architecture")]
public sealed class EndpointAuditTests
{
    // TODO-07: Start a NotesApiFactory, resolve EndpointDataSource from its Services and take the
    // RouteEndpoints. Every one must carry IAuthorizeData (RequireAuthorization, [Authorize]) or IAllowAnonymous
    // (AllowAnonymous) metadata: endpoint.Metadata.GetMetadata<T>(). Put the offenders' DisplayName in the
    // failure message.
    [TestMethod]
    public async Task EveryEndpointShouldRequireAuthorizationOrAllowAnonymousExplicitly()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }

    [TestMethod]
    public async Task TheAuditShouldSeeTheEndpointsItGuards()
    {
        // Guards the guard: if the list of endpoints were empty, the audit above would pass for the wrong
        // reason. Check that RoutePattern.RawText includes /notes/{id:int}, /admin/stats and /health.
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }
}
