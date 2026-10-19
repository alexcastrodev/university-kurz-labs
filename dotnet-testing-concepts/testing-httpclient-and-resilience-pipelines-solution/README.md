# Testing HttpClient Code and Resilience Pipelines (Solution)

Official solution for the `testing-httpclient-and-resilience-pipelines` lab. 13 tests in about 0.15 s,
although the pipeline under test retries with exponential delays, times attempts out after 2 s and opens
its circuit for 30 s. All of that happens on a `FakeTimeProvider`.

## What changed from the starter

- **`FakeFreightApi`** is the only test double, and it replaces only the last hop of the real chain
  (`ConfigurePrimaryHttpMessageHandler`). The idempotency handler and the resilience pipeline are the
  production ones. Three details make it honest:
  - an unscripted request fails the test, so a retry nobody scripted cannot pass as a 200;
  - each attempt is copied into a `SentRequest` inside `SendAsync`. The pipeline resends the same
    `HttpRequestMessage` instance (checked: one distinct instance for three attempts), so reading the
    stored messages afterwards would show the last attempt's headers three times;
  - the body is read with `CopyToAsync`, like a transport. With `ReadAsStringAsync` the content buffers
    itself and a non-replayable body looks replayable; a fake that never reads the body hides it too.
- **`IdempotencyKeyHandlerTests`** test the `DelegatingHandler` alone with an `HttpMessageInvoker` and an
  `InnerHandler`. The chain test (`EveryAttemptCarriesTheSameKeyAndTheSameBody`) proves the order:
  the key handler sits outside the resilience handler, so it runs once per call.
- **The fake clock reaches the pipeline through DI.** `AddResilienceHandler` registers the pipeline with
  Polly's `AddResiliencePipeline`, which uses the `TimeProvider` registered in the container when there is
  one. Registering `FakeTimeProvider` before building the provider is all it takes.
- **`RunAsync`** advances the fake clock in 250 ms steps until the call completes, up to 50 s of fake time.
  Nothing else moves that clock. A call that is simply awaited waits for `HttpClient.Timeout`, which is
  100 real seconds by default and does not use `TimeProvider` (seen while writing this lab: a probe call
  nobody drove took 1 min 40 s and ended in `TaskCanceledException`).
- **`DisposeAsync` drives the clock too.** Polly's pipeline waits for executions still in flight before
  it disposes, in 1 s steps on the pipeline's `TimeProvider`, giving up after 30 s. With a fake clock and a
  hanging call (a failing test) a plain `ServiceProvider.Dispose()` blocked the whole run forever. The test
  class is `IAsyncDisposable` and advances the clock while the provider disposes.
- **`ASlowAttemptTimesOutWithoutWaitingForReal`** uses `Hang()`, which awaits `Task.Delay(Infinite, token)`.
  A slow fake that ignores the token would keep the attempt alive after the timeout.
- **`ACancellationByTheCallerIsNotATimeout`** uses `Assert.ThrowsAsync<OperationCanceledException>`:
  `HttpClient` throws `TaskCanceledException`, a derived type, which `ThrowsExactlyAsync` would reject.
- **`TheCircuitOpensAfterRepeatedFailuresAndStopsCallingTheApi`**: the first call makes 3 failed attempts,
  the fourth failure (first attempt of the second call) reaches the minimum throughput of 4 with a 100%
  failure ratio and opens the circuit. The retry does not retry `BrokenCircuitException`, so the second call
  ends there, and the third never reaches the API: 4 attempts in total.

## Bonus notes

Mutation check, run against this solution. Each change was tried on its own, and `src` was compared with
the original afterwards (no differences):

| Change to the production code | Tests that fail |
| --- | --- |
| The retry handles nothing (`ShouldHandle` returns false) | 7 tests: retries, key and body, timeout, both circuit tests, manifest |
| `IdempotencyKeyHandler` registered after `AddResilienceHandler` | `EveryAttemptCarriesTheSameKeyAndTheSameBody` |
| No attempt timeout | `ASlowAttemptTimesOutWithoutWaitingForReal` |
| No circuit breaker | both circuit tests |
| No `LoadIntoBufferAsync` for the manifest | `AManifestFromAForwardOnlyStreamIsSentInFullOnEveryAttempt` |
| Every failure status mapped to `Unavailable` | `ARejectedRequestIsNotRetried` (both rows) |
| `catch (OperationCanceledException)` returns `Unavailable("timeout")` | `ACancellationByTheCallerIsNotATimeout` |
| `catch (JsonException)` returns `Unavailable` | `AMalformedQuoteIsAContractBugNotAnOutage` |

The naive versions were checked in scratch tests: headers read from the stored messages after the call
showed one key three times while the attempts had sent three different keys; a fake that never reads the
body let the forward-only manifest "succeed" on retry; a fake that reads with `ReadAsStringAsync` showed the
full body twice; reading with `CopyToAsync` made the retry fail with `InvalidOperationException: The stream
was already consumed. It cannot be read again.`

Not covered here: real sockets, TLS, proxies and HTTP/2 behavior. A fake handler never opens a connection.
For that, run a few tests against a real HTTP server, for example WireMock.Net (an in-process server on a
localhost port), and keep them few.
