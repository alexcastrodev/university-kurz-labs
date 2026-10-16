using Tenancy.Domain;

namespace Tenancy.Application.Tests;

// TODO-02: Test CreateTenantHandler with the FakeTenantRepository. Each test checks the status the use
// case returns, and what was or was not stored.
[TestClass]
// MSTest passes the TestContext of the running test to the constructor. Pass its CancellationToken to
// HandleAsync instead of CancellationToken.None: it is cancelled when the test times out or the run is aborted.
public sealed class CreateTenantHandlerTests(TestContext testContext)
{
    private readonly FakeTenantRepository _tenants = new();

    private Task<CreateTenantResult> CreateAsync(string subdomain) =>
        new CreateTenantHandler(_tenants).HandleAsync(subdomain, testContext.CancellationToken);
    
    [TestMethod]
    public async Task ShouldCreateAndStoreATenantWithTheNormalizedSubdomain()
    {
        await _tenants.AddAsync(new Tenant("Acme"), testContext.CancellationToken);
        Assert.HasCount(1, _tenants.Stored);
    }

    [TestMethod]
    public async Task ShouldReportAnInvalidSubdomainWithoutTouchingTheRepository()
    {
        await _tenants.AddAsync(new Tenant("Acme"), testContext.CancellationToken);
        
        var result = await CreateAsync(new string('a', 64));
        
        Assert.AreEqual(CreateTenantStatus.InvalidSubdomain, result.Status);
        Assert.HasCount(1, _tenants.Stored);
    }

    [TestMethod]
    public async Task ShouldRejectASubdomainThatIsAlreadyTakenIgnoringCase()
    {
        // Seed the fake with "acme", then ask for "Acme".
        await _tenants.AddAsync(new Tenant("acme"), testContext.CancellationToken);
        
        var result = await CreateAsync("Acme");
        
        Assert.AreEqual(CreateTenantStatus.SubdomainTaken, result.Status);
        Assert.HasCount(1, _tenants.Stored);
    }
}
