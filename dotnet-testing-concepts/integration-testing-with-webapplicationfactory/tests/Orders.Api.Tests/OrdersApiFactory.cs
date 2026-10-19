using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Orders.Api.Tests;

// The API as the tests see it: a lower order limit, and a fake inventory instead of the HTTP one.
// There is no `public partial class Program` in Program.cs: since .NET 10 a source generator makes the
// Program class of top-level statements public, so WebApplicationFactory<Program> compiles without it.
public sealed class OrdersApiFactory : WebApplicationFactory<Program>
{
    public const int MaxQuantity = 3;

    public FakeInventory Inventory { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // TODO-01: Two overrides.
        // 1. Set Orders:MaxQuantity to MaxQuantity. Read Program.cs first: when does it read that value?
        //    builder.UseSetting(key, value) and builder.ConfigureAppConfiguration(...) both look right.
        //    Only one of them reaches a value read before builder.Build().
        // 2. Replace IInventoryClient with Inventory, inside builder.ConfigureTestServices(services => ...).
        //    ConfigureTestServices runs after Program.cs registered its services. services.RemoveAll<T>()
        //    (Microsoft.Extensions.DependencyInjection.Extensions) removes the typed HttpClient first.
    }
}
