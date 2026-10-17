using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Tenancy.Api.Tenants;
using Tenancy.Application;

namespace Tenancy.Api.Tests;

// The whole stack on a real SQLite file: HTTP in, SQL out. Few tests, only for what crosses layers.
[TestClass]
[TestCategory("Integration")]
public sealed class TenantsApiTests(TestContext testContext)
{
    private readonly TestContext _testContext = testContext;
    private WebApplicationFactory<Program> _factory = null!;

    private string _db = null;
    // TODO-04: Start a new API before each test with a [TestInitialize] method, and stop it after each test
    // with a [TestCleanup] method. Point ConnectionStrings:Tenancy at a new SQLite file in
    // Path.GetTempPath() (WithWebHostBuilder + UseSetting) and delete the file in the cleanup.
    // Add Pooling=False to the connection string so closing a connection really closes the file.
    // Why per test? Tests run in parallel, and each one creates tenants. The factory has DisposeAsync:
    // a [TestCleanup] method may be async, so it can await it.

    [TestInitialize]
    public void Setup()
    {
        // Aqui o :N é como se fosse Guid.NewGuid().toString("N")
        _db = Path.Combine(Path.GetTempPath(), $"tenancy-{Guid.NewGuid():N}.db");
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("ConnectionStrings:Tenancy", $"Data Source={_db};Pooling=False"));
    }

    [TestCleanup]
    public async Task Teardown()
    {
        await _factory.DisposeAsync();
        File.Delete(_db);
    }

    private HttpClient ClientFor(string host)
    {
        var client = _factory.CreateClient();
        client.BaseAddress = new Uri($"http://{host}");
        return client;
    }

    // TODO-05: Write the tests below. Use ClientFor("localhost") to POST /tenants with a JSON body
    // { subdomain }, and ClientFor("acme.localhost") to reach a tenant by its subdomain.
    [TestMethod]
    public async Task ShouldCreateATenantWithTheNormalizedSubdomain()
    {
        // 201, and the DTO has the lower-case subdomain and a version 7 id.
        var request = new
        {
            subdomain = "AKatsUki"
        };
        
        // Act
        var response = await ClientFor("localhost").PostAsJsonAsync("/tenants", request, cancellationToken: _testContext.CancellationToken);
        var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();
        
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.AreEqual("akatsuki", tenant?.Subdomain);
        Assert.AreEqual(7, tenant?.Id.Version);
    }

    [TestMethod]
    public async Task ShouldReturn400WhenTheSubdomainIsInvalid()
    {
        var request = new
        {
            subdomain = "-AKatsUki"
        };
        
        // Act
        var response = await ClientFor("localhost").PostAsJsonAsync("/tenants", request, cancellationToken: _testContext.CancellationToken);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturn409WhenTheSubdomainIsAlreadyTaken()
    {
        var request = new
        {
            subdomain = "AKatsUki"
        };
        
        // Act
        await ClientFor("localhost").PostAsJsonAsync("/tenants", request, cancellationToken: _testContext.CancellationToken);
        var response = await ClientFor("localhost").PostAsJsonAsync("/tenants", request, cancellationToken: _testContext.CancellationToken);

        // Assert
        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldResolveTheCurrentTenantFromTheSubdomain()
    {
        // Create two tenants, then ask GET /tenants/current through the host of one of them.
        
        // Act
        await ClientFor("localhost").PostAsJsonAsync("/tenants", new
        {
            subdomain = "AKatsUki"
        }, cancellationToken: _testContext.CancellationToken);

        await ClientFor("localhost").PostAsJsonAsync("/tenants", new
        {
            subdomain = "taka"
        }, cancellationToken: _testContext.CancellationToken);

        var request = await ClientFor("taka").GetAsync("tenants/current", cancellationToken: _testContext.CancellationToken);
        var response = await request.Content.ReadFromJsonAsync<TenantDto>(_testContext.CancellationToken);
        
        // Assert
        Assert.AreEqual("taka", response?.Subdomain);
    }

    [TestMethod]
    public async Task ShouldReturn404ForCurrentWhenThereIsNoSuchTenant()
    {
        // Two hosts, same answer: a subdomain that is not a tenant, and no subdomain at all. Use [DataRow].
        await ClientFor("localhost").PostAsJsonAsync("/tenants", new
        {
            subdomain = "AKatsUki"
        }, cancellationToken: _testContext.CancellationToken);

        await ClientFor("localhost").PostAsJsonAsync("/tenants", new
        {
            subdomain = "taka"
        }, cancellationToken: _testContext.CancellationToken);

        var request = await ClientFor("taka").GetAsync("tenants/current", cancellationToken: _testContext.CancellationToken);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, request.StatusCode);
    }

    [TestMethod]
    public async Task ShouldEnforceUniqueSubdomainsInTheDatabaseItself()
    {
        // Skip the handler: resolve TenancyDbContext from _factory.Services in a scope and add a
        // duplicate Tenant directly. The handler's check is a courtesy; what does the database say?
        
        
    }
}
