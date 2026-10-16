using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorDataAccessLab.Tests;

/// <summary>Holds the page in flight until the test lets it go. No delays involved.</summary>
public sealed class HeldGate : ILoadGate
{
    private readonly TaskCompletionSource _started = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _release = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Started => _started.Task;

    public Task WaitAsync()
    {
        _started.TrySetResult();
        return _release.Task;
    }

    public void Release() => _release.TrySetResult();
}

public class ProductTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private HttpClient ClientWith(HeldGate gate) =>
        factory.WithWebHostBuilder(b => b.ConfigureServices(s => s.AddSingleton<ILoadGate>(gate))).CreateClient();

    private static List<string> Names(IEnumerable<Product> products) => products.Select(p => p.Name).ToList();

    [Fact]
    public void ShouldResolveFactoryAndServiceFromContainer()
    {
        using var scope = factory.Services.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<IDbContextFactory<ShopContext>>());
        Assert.NotNull(scope.ServiceProvider.GetService<IProductService>());
        Assert.NotNull(scope.ServiceProvider.GetService<SqliteConnection>());
    }

    [Fact]
    public async Task ShouldListProductsOrderedByName()
    {
        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProductService>();

        var products = await service.ListAsync();

        Assert.Equal(["Eraser", "Notebook", "Stapler"], Names(products));
    }

    [Fact]
    public async Task ShouldUseOneNewDisposedContextPerListCallAndLeaveNothingTracked()
    {
        var created = new List<SpyContext>();
        var keeper = factory.Services.GetRequiredService<SqliteConnection>();
        var options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(keeper.ConnectionString).Options;
        var service = new DbProductService(new SpyFactory(options, created));

        await service.ListAsync();
        await service.ListAsync();

        Assert.Equal(2, created.Count);
        Assert.All(created, c => Assert.True(c.Disposed));
        Assert.All(created, c => Assert.Equal(0, c.TrackedAtDispose));
    }

    [Fact]
    public async Task ShouldRunOverlappingQueriesEachOnItsOwnContext()
    {
        using var scope = factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProductService>();

        var counts = await service.CountConcurrentlyAsync(8);

        Assert.Equal(8, counts.Length);
        Assert.All(counts, c => Assert.Equal(3, c));
    }

    [Fact]
    public async Task ShouldSendLoadingFirstAndDataLaterWhenStreamRendering()
    {
        var gate = new HeldGate();
        var client = ClientWith(gate);
        // Watchdog only: if streaming is off the headers never arrive and the test fails instead of hanging.
        using var watchdog = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        using var response = await client.GetAsync("/products", HttpCompletionOption.ResponseHeadersRead, watchdog.Token);
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(watchdog.Token));
        var firstFlush = new System.Text.StringBuilder();
        var buffer = new char[4096];
        while (!firstFlush.ToString().Contains("Loading..."))
        {
            var read = await reader.ReadAsync(buffer.AsMemory(), watchdog.Token);
            if (read == 0) break;
            firstFlush.Append(buffer, 0, read);
        }

        Assert.Contains("Loading...", firstFlush.ToString());
        Assert.DoesNotContain("Stapler", firstFlush.ToString());

        gate.Release();
        var all = firstFlush + await reader.ReadToEndAsync(watchdog.Token);

        Assert.Contains("Stapler", all);
    }

    [Fact]
    public async Task ShouldContainPlaceholderStreamingUpdateAndDataInFullBody()
    {
        var gate = new HeldGate();
        var client = ClientWith(gate);

        var request = client.GetStringAsync("/products");
        await Task.WhenAny(gate.Started, request);
        gate.Release();
        var body = await request;

        Assert.Contains("Loading...", body);
        Assert.Contains("blazor-ssr", body);
        Assert.Contains("Eraser", body);
        Assert.Contains("Stapler", body);
    }

    [Fact]
    public async Task ShouldWaitForDataBeforeSendingAnythingWithoutStreamRendering()
    {
        var gate = new HeldGate();
        var client = ClientWith(gate);

        var request = client.GetAsync("/products-plain", HttpCompletionOption.ResponseHeadersRead);
        await Task.WhenAny(gate.Started, request);

        Assert.False(request.IsCompleted);

        gate.Release();
        using var response = await request;
        var body = await response.Content.ReadAsStringAsync();

        Assert.Contains("Stapler", body);
        Assert.DoesNotContain("Loading...", body);
        Assert.DoesNotContain("blazor-ssr", body);
    }

    [Fact]
    public async Task ShouldExposeSameListAsJson()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products");
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(["Eraser", "Notebook", "Stapler"], Names(products!));
    }

    [Fact]
    public async Task ShouldSeeRenameFromAnotherContext()
    {
        // Own host: this test mutates data.
        using var own = new WebApplicationFactory<Program>();
        using var scope = own.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IProductService>();

        var renamed = await service.RenameAsync(1, "Marker");
        var missing = await service.RenameAsync(999, "Nope");
        var names = Names(await service.ListAsync());

        Assert.True(renamed);
        Assert.False(missing);
        Assert.Contains("Marker", names);
        Assert.DoesNotContain("Stapler", names);
    }

    private sealed class SpyContext(DbContextOptions<ShopContext> options) : ShopContext(options)
    {
        public bool Disposed { get; private set; }
        public int TrackedAtDispose { get; private set; } = -1;

        public override ValueTask DisposeAsync()
        {
            Record();
            return base.DisposeAsync();
        }

        public override void Dispose()
        {
            Record();
            base.Dispose();
        }

        private void Record()
        {
            if (Disposed) return;
            TrackedAtDispose = ChangeTracker.Entries().Count();
            Disposed = true;
        }
    }

    private sealed class SpyFactory(DbContextOptions<ShopContext> options, List<SpyContext> created)
        : IDbContextFactory<ShopContext>
    {
        public ShopContext CreateDbContext()
        {
            var db = new SpyContext(options);
            created.Add(db);
            return db;
        }
    }
}
