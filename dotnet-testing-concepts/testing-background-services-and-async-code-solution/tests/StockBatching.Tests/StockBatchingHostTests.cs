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
        builder.Logging.AddFakeLogging();
        builder.Services.AddSingleton<TimeProvider>(_time); // before AddStockBatching, so it wins
        builder.Services.AddSingleton<IStockLedger>(_ledger);
        builder.Services.AddStockBatching(new StockBatchOptions { Interval = Interval });
        _host = builder.Build();
    }

    private CancellationToken Token => _testContext.CancellationToken;

    private ChannelWriter<StockMovement> Writer => _host.Services.GetRequiredService<ChannelWriter<StockMovement>>();

    private async Task StartAsync()
    {
        await _host.StartAsync(Token);
        await _time.TimerCreated.WaitAsync(TimeSpan.FromSeconds(5), Token);
    }

    public void Dispose() => _host.Dispose();

    [TestMethod]
    public async Task TheHostRunsTheWorkerAndDrainsTheQueueOnShutdown()
    {
        await StartAsync();
        await Writer.WriteAsync(new("KB-01", 5), Token);
        _time.Advance(Interval);
        Assert.HasCount(1, await _ledger.NextCallAsync(Token));

        await Writer.WriteAsync(new("MS-02", -1), Token);
        await _host.StopAsync(Token);

        var drained = await _ledger.NextCallAsync(Token);
        Assert.AreEqual(new StockMovement("MS-02", -1), Assert.ContainsSingle(drained));
    }

    [TestMethod]
    public async Task AFailingLedgerDoesNotStopTheHost()
    {
        var lifetime = _host.Services.GetRequiredService<IHostApplicationLifetime>();
        await StartAsync();
        _ledger.FailNext(1);
        await Writer.WriteAsync(new("KB-01", 5), Token);

        _time.Advance(Interval);
        await _ledger.NextCallAsync(Token);
        _time.Advance(Interval);
        await _ledger.NextCallAsync(Token); // the retry: the worker survived the failure

        Assert.IsFalse(lifetime.ApplicationStopping.IsCancellationRequested);
        var logs = _host.Services.GetFakeLogCollector().GetSnapshot();
        Assert.ContainsSingle(logs.Where(r => r.Level == LogLevel.Error && r.Category == typeof(StockBatchWorker).FullName));
        Assert.IsFalse(logs.Any(r => r.Level == LogLevel.Critical));
        await _host.StopAsync(Token);
    }
}
