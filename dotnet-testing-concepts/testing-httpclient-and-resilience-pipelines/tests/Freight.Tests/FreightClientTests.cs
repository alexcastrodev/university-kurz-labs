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
    private readonly ServiceProvider? _services;
    private readonly FreightClient? _client;

    public FreightClientTests(TestContext testContext)
    {
        _testContext = testContext;
        // TODO-02: Build a ServiceCollection. Register _time as the TimeProvider (the resilience pipeline
        // resolves it from the container), call AddFreightClient(new Uri("http://freight.test/")), and on
        // the IHttpClientBuilder it returns replace only the last hop: ConfigurePrimaryHttpMessageHandler(() => _api).
        // Build the provider into _services and resolve FreightClient into _client.
        _services = null;
        _client = null;
    }

    [TestInitialize]
    public void TheClientIsWired() => Assert.IsNotNull(_client, "TODO-02: build the client in the constructor first.");

    private CancellationToken Token => _testContext.CancellationToken;

    // Polly waits for executions still in flight before it disposes a pipeline, and it waits on the
    // pipeline's TimeProvider, which is the fake clock. A test that failed with a call still hanging would
    // hang the whole run in a plain Dispose. Polly gives up after 30 seconds, so drive the clock past that.
    public async ValueTask DisposeAsync()
    {
        if (_services is not null)
        {
            await DriveAsync(_services.DisposeAsync().AsTask());
        }
    }

    // Retry delays and timeouts wait on the fake clock, and nothing moves it but the test. Run every call
    // through this helper. A call you simply await waits for HttpClient.Timeout instead: 100 real seconds
    // by default, because that one does not use TimeProvider.
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
        // TODO-03: One 200 with QuoteJson. Assert the parsed result, and what was sent: one POST to
        // /quotes with the request serialized as camelCase JSON.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    [DataRow(HttpStatusCode.BadRequest)]
    [DataRow(HttpStatusCode.UnprocessableEntity)]
    public async Task ARejectedRequestIsNotRetried(HttpStatusCode status)
    {
        // TODO-03: Rejected with "status NNN", and exactly one attempt.
        await Task.CompletedTask;
        Assert.Fail($"TODO-03: write this test ({status}).");
    }

    [TestMethod]
    public async Task ATransientFailureIsRetriedUntilItSucceeds()
    {
        // TODO-04: 503, 429, then 200. Ok, AND three attempts. Without the attempt count, a fake that
        // always answers 200 passes this test with the retry deleted.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task WhenTheRetriesRunOutTheApiIsUnavailable()
    {
        // TODO-04: Three 503s: Unavailable "status 503" after three attempts.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task EveryAttemptCarriesTheSameKeyAndTheSameBody()
    {
        // TODO-04: 503, 503, 200. All three attempts carry one and the same Idempotency-Key, and the same body.
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ASlowAttemptTimesOutWithoutWaitingForReal()
    {
        // TODO-05: Three Hang() attempts. Unavailable "timeout", three attempts, and at least 6 seconds
        // passed on the FAKE clock (_time.GetUtcNow()). The test itself takes milliseconds.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task ACancellationByTheCallerIsNotATimeout()
    {
        // TODO-05: Hang(), start the call with a token you cancel yourself (link it to Token), cancel it.
        // The caller gets an OperationCanceledException (HttpClient throws the derived
        // TaskCanceledException: which Assert method accepts a derived type?), not a result.
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task TheCircuitOpensAfterRepeatedFailuresAndStopsCallingTheApi()
    {
        // TODO-06: Read the breaker options in AddFreightClient (ratio 0.5, minimum throughput 4). Script
        // four 500s and make three calls. First: "status 500". Second and third: "circuit open". The API
        // saw four attempts in total: the third call never reached it.
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    [TestMethod]
    public async Task TheCircuitLetsAProbeThroughAfterTheBreakDuration()
    {
        // TODO-08 (optional): Open the circuit as above, advance the fake clock by the break duration,
        // script a 200: the next call is let through as a probe and succeeds.
        await Task.CompletedTask;
        Assert.Inconclusive("TODO-08 (optional)");
    }

    [TestMethod]
    public async Task AMalformedQuoteIsAContractBugNotAnOutage()
    {
        // TODO-07: A 200 whose "price" is a string. JsonException reaches the caller
        // (Assert.ThrowsExactlyAsync), and there was only one attempt.
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }

    [TestMethod]
    public async Task AManifestFromAForwardOnlyStreamIsSentInFullOnEveryAttempt()
    {
        // TODO-07: UploadManifestAsync with a ForwardOnlyStream, scripted 503 then 200. Ok, two PUTs to
        // /shipments/S-42/manifest, and BOTH carry the full CSV. This only proves something if your fake
        // reads the body with CopyToAsync.
        await Task.CompletedTask;
        Assert.Fail("TODO-07: write this test.");
    }
}
