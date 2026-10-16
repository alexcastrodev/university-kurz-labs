using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace HttpClientLab.Tests;

public sealed class GamesClientTests
{
    private static readonly Uri BaseAddress = new("http://games.test");

    private static ServiceProvider Build(FakeHandler fake, TimeSpan? timeout = null)
    {
        var services = new ServiceCollection();
        services.AddGamesClient(BaseAddress, timeout)
            .ConfigurePrimaryHttpMessageHandler(() => fake);
        return services.BuildServiceProvider();
    }

    private static GamesClient Client(ServiceProvider sp) => sp.GetRequiredService<GamesClient>();

    [Fact]
    public async Task ShouldUseTheConfiguredBaseAddress()
    {
        var fake = FakeHandler.Json(new Game(1, "Zelda"));
        using var sp = Build(fake);

        var game = await Client(sp).GetGameAsync(1);

        Assert.Equal(new Game(1, "Zelda"), game);
        Assert.Equal(new Uri("http://games.test/games/1"), fake.Requests.Single().RequestUri);
    }

    [Fact]
    public async Task ShouldSendACorrelationIdOnEveryRequest()
    {
        var fake = FakeHandler.Json(new Game(1, "Zelda"));
        using var sp = Build(fake);

        await Client(sp).GetGameAsync(1);

        Assert.True(fake.Requests.Single().Headers.Contains(CorrelationIdHandler.HeaderName),
            "the DelegatingHandler must add X-Correlation-Id and be registered with AddHttpMessageHandler");
    }

    [Fact]
    public async Task ShouldKeepACorrelationIdThatTheCallerAlreadySet()
    {
        var handler = new CorrelationIdHandler { InnerHandler = FakeHandler.Json(new Game(1, "Zelda")) };
        using var invoker = new HttpMessageInvoker(handler);
        using var request = new HttpRequestMessage(HttpMethod.Get, "http://games.test/x");
        request.Headers.Add(CorrelationIdHandler.HeaderName, "mine");

        await invoker.SendAsync(request, CancellationToken.None);

        Assert.Equal(["mine"], request.Headers.GetValues(CorrelationIdHandler.HeaderName));
    }

    [Fact]
    public void ShouldRotateHandlersEveryFiveMinutes()
    {
        using var sp = Build(FakeHandler.Json(new Game(1, "x")));

        var options = sp.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(nameof(GamesClient));

        Assert.Equal(TimeSpan.FromMinutes(5), options.HandlerLifetime);
    }

    [Fact]
    public void ShouldLetPooledConnectionsExpireSoDnsChangesAreSeen()
    {
        var services = new ServiceCollection();
        services.AddGamesClient(BaseAddress);
        using var sp = services.BuildServiceProvider();
        var name = nameof(GamesClient);
        var builder = sp.GetRequiredService<HttpMessageHandlerBuilder>();
        builder.Name = name;
        foreach (var action in sp.GetRequiredService<IOptionsMonitor<HttpClientFactoryOptions>>().Get(name).HttpMessageHandlerBuilderActions)
        {
            action(builder);
        }

        var primary = Assert.IsType<SocketsHttpHandler>(builder.PrimaryHandler);
        Assert.Equal(TimeSpan.FromMinutes(2), primary.PooledConnectionLifetime);
    }

    [Fact]
    public async Task ShouldReturnNullWhenTheGameDoesNotExist()
    {
        using var sp = Build(FakeHandler.Status(HttpStatusCode.NotFound));

        Assert.Null(await Client(sp).GetGameAsync(404));
    }

    [Fact]
    public async Task ShouldStillThrowForServerErrors()
    {
        using var sp = Build(FakeHandler.Status(HttpStatusCode.ServiceUnavailable));

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => Client(sp).GetGameAsync(1));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, ex.StatusCode);
    }

    [Fact]
    public async Task ShouldMapHttpClientTimeoutToGameServiceTimeoutException()
    {
        using var sp = Build(FakeHandler.Hanging(), TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAsync<GameServiceTimeoutException>(() => Client(sp).GetGameAsync(1));
    }

    [Fact]
    public async Task ShouldNotHideCancellationRequestedByTheCaller()
    {
        using var sp = Build(FakeHandler.Hanging());
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        var ex = await Record.ExceptionAsync(() => Client(sp).GetGameAsync(1, cts.Token));

        Assert.IsAssignableFrom<OperationCanceledException>(ex);
    }

    [Fact]
    public async Task ShouldReturnTheFirstLineWithoutWaitingForTheWholeBody()
    {
        var gate = new TaskCompletionSource();
        var fake = new FakeHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(new GatedStream("first line\n"u8.ToArray(), gate.Task))
        }));
        using var sp = Build(fake);

        var line = await Client(sp).ReadFirstLineAsync("/big").WaitAsync(TimeSpan.FromSeconds(3));
        gate.SetResult();

        Assert.Equal("first line", line);
    }

    private sealed class GatedStream(byte[] first, Task gate) : Stream
    {
        private bool _firstSent;

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (!_firstSent)
            {
                _firstSent = true;
                first.CopyTo(buffer);
                return first.Length;
            }

            await gate.WaitAsync(cancellationToken);
            return 0;
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
