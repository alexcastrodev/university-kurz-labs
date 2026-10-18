using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tenancy.Api.Tenants;
using Tenancy.Application;
using Tenancy.Domain;
using Tenancy.Infrastructure.Persistence;

namespace Tenancy.Api.Tests;

// The whole stack on a real SQLite file: HTTP in, SQL out. Few tests, only for what crosses layers.
[TestClass]
[TestCategory("Integration")]
public sealed class TenantsApiTests(TestContext testContext)
{
    private WebApplicationFactory<Program> _factory = null!;

    private Task<HttpResponseMessage> CreateTenantAsync(string subdomain) =>
        ClientFor("localhost").PostAsJsonAsync("/tenants", new { subdomain }, testContext.CancellationToken);

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
        var response = await CreateTenantAsync("AKatsUki");
        
        // Act
        var tenant = await response.Content.ReadFromJsonAsync<TenantDto>();
        
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.AreEqual("akatsuki", tenant?.Subdomain);
        Assert.AreEqual(7, tenant?.Id.Version);
    }

    [TestMethod]
    public async Task ShouldReturn400WhenTheSubdomainIsInvalid()
    {
        var response = await CreateTenantAsync("-AKatsUki");
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturn409WhenTheSubdomainIsAlreadyTaken()
    {
        await CreateTenantAsync("AKatsUki");
        var sameTenantRequest = await CreateTenantAsync("AKatsUki");

        // Assert
        Assert.AreEqual(HttpStatusCode.Conflict, sameTenantRequest.StatusCode);
    }

    [TestMethod]
    public async Task ShouldResolveTheCurrentTenantFromTheSubdomain()
    {
        // Create two tenants, then ask GET /tenants/current through the host of one of them.
        
        // Act
        await CreateTenantAsync("AKatsUki");
        await CreateTenantAsync("taka");

        var request = await ClientFor("taka.localhost").GetAsync("tenants/current", cancellationToken: testContext.CancellationToken);
        var response = await request.Content.ReadFromJsonAsync<TenantDto>(testContext.CancellationToken);
        
        // Assert
        Assert.AreEqual("taka", response!.Subdomain);
    }

    [TestMethod]
    [DataRow("AKatsUki")]
    [DataRow("taka")]
    public async Task ShouldReturn404ForCurrentWhenThereIsNoSuchTenant(string host)
    {
        // Two hosts, same answer: a subdomain that is not a tenant, and no subdomain at all. Use [DataRow].
        var response = await ClientFor(host).GetAsync("tenants/current", cancellationToken: testContext.CancellationToken);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldEnforceUniqueSubdomainsInTheDatabaseItself()
    {
        // Skip the handler: resolve TenancyDbContext from _factory.Services in a scope and add a
        // duplicate Tenant directly. The handler's check is a courtesy; what does the database say?
        await CreateTenantAsync("acme");
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TenancyDbContext>();

        db.Tenants.Add(new Tenant("acme"));
        
        // Assert
        await Assert.ThrowsExactlyAsync<DbUpdateException>(() => db.SaveChangesAsync(testContext.CancellationToken));
    }
}
