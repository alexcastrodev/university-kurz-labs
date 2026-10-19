using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Notes.Api.Tests;

// Happy-path tests only visit the endpoints someone remembered. This test visits every endpoint the app mapped.
[TestClass]
[TestCategory("Architecture")]
public sealed class EndpointAuditTests
{
    private static IReadOnlyList<RouteEndpoint> Endpoints(NotesApiFactory api) =>
        [.. api.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>()];

    [TestMethod]
    public async Task EveryEndpointShouldRequireAuthorizationOrAllowAnonymousExplicitly()
    {
        await using var api = new NotesApiFactory();

        var undecided = Endpoints(api)
            .Where(e => e.Metadata.GetMetadata<IAuthorizeData>() is null
                     && e.Metadata.GetMetadata<IAllowAnonymous>() is null)
            .Select(e => e.DisplayName)
            .ToList();

        Assert.IsEmpty(undecided, $"No authorization decision on: {string.Join(", ", undecided)}");
    }

    [TestMethod]
    public async Task TheAuditShouldSeeTheEndpointsItGuards()
    {
        // Guards the guard: an empty list of endpoints would make the audit pass for the wrong reason.
        await using var api = new NotesApiFactory();

        var routes = Endpoints(api).Select(e => e.RoutePattern.RawText).ToList();

        Assert.Contains("/notes/{id:int}", routes);
        Assert.Contains("/admin/stats", routes);
        Assert.Contains("/health", routes);
    }
}
