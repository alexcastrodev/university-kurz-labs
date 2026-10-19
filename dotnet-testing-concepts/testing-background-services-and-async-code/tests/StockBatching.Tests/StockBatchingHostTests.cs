using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace StockBatching.Tests;

// The same worker inside a real generic host: the registration, the host lifecycle and what the host
// does when a BackgroundService fails (BackgroundServiceExceptionBehavior.StopHost by default).
[TestClass]
public sealed class StockBatchingHostTests : IDisposable
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    private readonly RecordingLedger _ledger = new();
    private readonly SignalingTimeProvider _time = new();
    private readonly TestContext _testContext;
    private readonly IHost _host;

    public StockBatchingHostTests(TestContext testContext)
    {
        _testContext = testContext;
        var builder = Host.CreateApplicationBuilder();
        builder.Logging.ClearProviders();
        builder.Logging.AddFakeLogging(); // read it back with _host.Services.GetFakeLogCollector()
        builder.Services.AddSingleton<TimeProvider>(_time); // before AddStockBatching, so it wins
        builder.Services.AddSingleton<IStockLedger>(_ledger);
        builder.Services.AddStockBatching(new StockBatchOptions { Interval = Interval });
        _host = builder.Build();
    }

    private CancellationToken Token => _testContext.CancellationToken;

    private ChannelWriter<StockMovement> Writer => _host.Services.GetRequiredService<ChannelWriter<StockMovement>>();

    public void Dispose() => _host.Dispose();

    [TestMethod]
    public async Task TheHostRunsTheWorkerAndDrainsTheQueueOnShutdown()
    {
        // TODO-05: Start the host and wait for the timer. Write one movement, tick, and see it applied.
        // Write another one and stop the host: it must be applied during shutdown.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task AFailingLedgerDoesNotStopTheHost()
    {
        // TODO-05: The ledger fails once. After the retry on the next tick, IHostApplicationLifetime's
        // ApplicationStopping must not be cancelled, the host log must hold one Error from the worker's
        // category and no Critical entry (the host writes a Critical one when it stops for a failed worker).
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }
}
