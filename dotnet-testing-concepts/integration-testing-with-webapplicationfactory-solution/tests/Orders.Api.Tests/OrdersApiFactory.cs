using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orders.Api.Orders;

namespace Orders.Api.Tests;

// The API as the tests see it: a lower order limit, and a fake inventory instead of the HTTP one.
public sealed class OrdersApiFactory : WebApplicationFactory<Program>
{
    public const int MaxQuantity = 3;

    public FakeInventory Inventory { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Program.cs reads Orders:MaxQuantity before Build. UseSetting reaches that read;
        // ConfigureAppConfiguration would only be visible after Build, too late for it.
        builder.UseSetting("Orders:MaxQuantity", MaxQuantity.ToString());

        // Runs after Program.cs registered its services, so the fake replaces the typed HttpClient.
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IInventoryClient>();
            services.AddSingleton<IInventoryClient>(Inventory);
        });
    }
}
