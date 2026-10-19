using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace Freight.Tests;

// The typed client with the real handler chain and the real resilience pipeline. Only the last hop is
// fake (FakeFreightApi), and the clock is fake, so retries and timeouts cost no real time.
[TestClass]
public sealed class FreightClientTests : IAsyncDisposable
{
    private static readonly QuoteRequest Request = new("1000-001", "4000-002", 2.5m);
    private const string QuoteJson = """{"price":12.40,"currency":"EUR","days":2}""";

    private readonly TestContext _testContext;
    private readonly FakeTimeProvider _time = new();
    private readonly FakeFreightApi _api = new();
    private readonly ServiceProvider _services;
    private readonly FreightClient _client;

    public FreightClientTests(TestContext testContext)
    {
        _testContext = testContext;
        var services = new ServiceCollection();
        services.AddSingleton<TimeProvider>(_time); // the resilience pipeline picks it up from here
        services.AddFreightClient(new Uri("http://freight.test/"))
            .ConfigurePrimaryHttpMessageHandler(() => _api); // replaces only the last hop
        _services = services.BuildServiceProvider();
        _client = _services.GetRequiredService<FreightClient>();
    }

    private CancellationToken Token => _testContext.CancellationToken;

    // Polly waits for executions still in flight before it disposes a pipeline, and it waits on the
    // pipeline's TimeProvider, which is the fake clock. A test that failed with a call still hanging would
    // hang the whole run in a plain Dispose. Polly gives up after 30 seconds, so drive the clock past that.
    public async ValueTask DisposeAsync() => await DriveAsync(_services.DisposeAsync().AsTask());

    // Retry delays and timeouts wait on the fake clock, and nothing moves it but the test. Advance it in
    // small steps until the task completes. A call nobody drives would wait for HttpClient.Timeout,
    // 100 real seconds by default, because that one does not use TimeProvider.
    private async Task<T> RunAsync<T>(Task<T> call)
    {
        await DriveAsync(call);
        Assert.IsTrue(call.IsCompleted, "The call did not complete after 50 seconds of fake time.");
        return await call;
    }

    private async Task DriveAsync(Task task)
    {
        for (var step = 0; step < 200 && !task.IsCompleted; step++)
        {
            _time.Advance(TimeSpan.FromMilliseconds(250));
            await Task.WhenAny(task, Task.Delay(1));
        }
    }

    [TestMethod]
    public async Task AQuoteIsReadFromA200()
    {
        _api.Respond(HttpStatusCode.OK, QuoteJson);

        var result = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightResult.Ok(new Quote(12.40m, "EUR", 2)), result);
        var sent = Assert.ContainsSingle(_api.Sent);
        Assert.AreEqual(HttpMethod.Post, sent.Method);
        Assert.AreEqual("/quotes", sent.Path);
        Assert.AreEqual("""{"fromZip":"1000-001","toZip":"4000-002","weightKg":2.5}""", sent.Body);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.BadRequest)]
    [DataRow(HttpStatusCode.UnprocessableEntity)]
    public async Task ARejectedRequestIsNotRetried(HttpStatusCode status)
    {
        _api.Respond(status);

        var result = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightResult.Rejected($"status {(int)status}"), result);
        Assert.HasCount(1, _api.Sent);
    }

    [TestMethod]
    public async Task ATransientFailureIsRetriedUntilItSucceeds()
    {
        _api.Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.TooManyRequests).Respond(HttpStatusCode.OK, QuoteJson);

        var result = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightOutcome.Ok, result.Outcome);
        Assert.HasCount(3, _api.Sent); // without this line a fake that always answers 200 passes too
    }

    [TestMethod]
    public async Task WhenTheRetriesRunOutTheApiIsUnavailable()
    {
        _api.Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.ServiceUnavailable);

        var result = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightResult.Unavailable("status 503"), result);
        Assert.HasCount(3, _api.Sent);
    }

    [TestMethod]
    public async Task EveryAttemptCarriesTheSameKeyAndTheSameBody()
    {
        _api.Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.OK, QuoteJson);

        await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.HasCount(3, _api.Sent);
        Assert.IsNotNull(_api.Sent[0].IdempotencyKey);
        Assert.HasCount(1, _api.Sent.Select(s => s.IdempotencyKey).Distinct());
        Assert.HasCount(1, _api.Sent.Select(s => s.Body).Distinct());
    }

    [TestMethod]
    public async Task ASlowAttemptTimesOutWithoutWaitingForReal()
    {
        _api.Hang().Hang().Hang();
        var start = _time.GetUtcNow();

        var result = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightResult.Unavailable("timeout"), result);
        Assert.HasCount(3, _api.Sent);
        Assert.IsGreaterThanOrEqualTo(TimeSpan.FromSeconds(6), _time.GetUtcNow() - start); // three 2 s attempts, on the fake clock
    }

    [TestMethod]
    public async Task ACancellationByTheCallerIsNotATimeout()
    {
        _api.Hang();
        using var caller = CancellationTokenSource.CreateLinkedTokenSource(Token);

        var call = _client.GetQuoteAsync(Request, caller.Token);
        caller.Cancel();

        // HttpClient throws TaskCanceledException, which derives from OperationCanceledException:
        // ThrowsAsync accepts derived types, ThrowsExactlyAsync<OperationCanceledException> would not.
        await Assert.ThrowsAsync<OperationCanceledException>(() => call);
        Assert.IsLessThanOrEqualTo(1, _api.Sent.Count);
    }

    [TestMethod]
    public async Task TheCircuitOpensAfterRepeatedFailuresAndStopsCallingTheApi()
    {
        for (var i = 0; i < 4; i++)
        {
            _api.Respond(HttpStatusCode.InternalServerError);
        }

        var first = await RunAsync(_client.GetQuoteAsync(Request, Token));  // 3 attempts, all failed
        var second = await RunAsync(_client.GetQuoteAsync(Request, Token)); // the 4th failure opens it
        var third = await RunAsync(_client.GetQuoteAsync(Request, Token));  // never reaches the API

        Assert.AreEqual(FreightResult.Unavailable("status 500"), first);
        Assert.AreEqual(FreightResult.Unavailable("circuit open"), second);
        Assert.AreEqual(FreightResult.Unavailable("circuit open"), third);
        Assert.HasCount(4, _api.Sent);
    }

    // TODO-08 (optional) in the starter.
    [TestMethod]
    public async Task TheCircuitLetsAProbeThroughAfterTheBreakDuration()
    {
        for (var i = 0; i < 4; i++)
        {
            _api.Respond(HttpStatusCode.InternalServerError);
        }

        await RunAsync(_client.GetQuoteAsync(Request, Token));
        await RunAsync(_client.GetQuoteAsync(Request, Token));
        _time.Advance(TimeSpan.FromSeconds(30));
        _api.Respond(HttpStatusCode.OK, QuoteJson);

        var probe = await RunAsync(_client.GetQuoteAsync(Request, Token));

        Assert.AreEqual(FreightOutcome.Ok, probe.Outcome);
        Assert.HasCount(5, _api.Sent);
    }

    [TestMethod]
    public async Task AMalformedQuoteIsAContractBugNotAnOutage()
    {
        _api.Respond(HttpStatusCode.OK, """{"price":"cheap","currency":"EUR","days":2}""");

        await Assert.ThrowsExactlyAsync<JsonException>(() => RunAsync(_client.GetQuoteAsync(Request, Token)));
        Assert.HasCount(1, _api.Sent);
    }

    [TestMethod]
    public async Task AManifestFromAForwardOnlyStreamIsSentInFullOnEveryAttempt()
    {
        const string csv = "sku,quantity\nKB-01,5\n";
        _api.Respond(HttpStatusCode.ServiceUnavailable).Respond(HttpStatusCode.OK);

        var result = await RunAsync(_client.UploadManifestAsync("S-42", new ForwardOnlyStream(csv), Token));

        Assert.AreEqual(FreightOutcome.Ok, result.Outcome);
        Assert.HasCount(2, _api.Sent);
        Assert.IsTrue(_api.Sent.All(s => s.Method == HttpMethod.Put && s.Path == "/shipments/S-42/manifest" && s.Body == csv));
    }
}
