using Tenancy.Domain;

namespace Tenancy.Application.Tests;

[TestClass]
public sealed class ResolveTenantHandlerTests(TestContext testContext)
{

    public static IEnumerable<(string Host, TenantResolutionStatus Expected)> Hosts =>
    [
        ("acme.example.com", TenantResolutionStatus.Found),
        ("ACmE.example.com", TenantResolutionStatus.Found),
        ("empty.example.com", TenantResolutionStatus.NotFound),
        ("localhost", TenantResolutionStatus.NoSubdomain),
    ];
    
    // TODO-03: A table of hosts and the status each one must produce. Make it a public static property
    // returning IEnumerable<(string Host, TenantResolutionStatus Expected)>. Cover a known tenant, the
    // same tenant in upper case, an unknown tenant and a host with no subdomain ("localhost").
    // Then point [DynamicData(nameof(...))] at it from the test below, and give the test the two parameters.
    [TestMethod]
    [DynamicData(nameof(Hosts))]
    public async Task ShouldResolveTheTenantFromTheFirstLabelOfTheHost(string host, TenantResolutionStatus expected)
    {
        var tenants = new FakeTenantRepository();
        tenants.Stored.Add(new Tenant("acme"));

        var result = await new ResolveTenantHandler(tenants).HandleAsync(host, testContext.CancellationToken);
        Assert.AreEqual(expected, result.Status);
    }

    [TestMethod]
    public async Task ShouldReturnTheStoredTenantWhenFound()
    {
        // Same instance, not just an equal-looking one: Assert.AreSame.
        var tenants = new FakeTenantRepository();
        var tenant = new Tenant("akatsuki");
        // refer to the same object instance, not create a new Tenant with same subdomain
        tenants.Stored.Add(tenant);
        var result = await new ResolveTenantHandler(tenants).HandleAsync("akatsuki.example.com", testContext.CancellationToken);
        
        Assert.AreSame(tenant, result.Tenant);
    }
}
