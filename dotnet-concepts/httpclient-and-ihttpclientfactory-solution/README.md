# HttpClient and IHttpClientFactory (Solution)

## Goal

Register a typed client the way production code should and handle a 404, a
timeout and a cancellation without network access. See the starter project for the
exercise statement.

## Solution notes

- `AddGamesClient` chains everything on the `IHttpClientBuilder`: base address and
  timeout, `AddHttpMessageHandler<CorrelationIdHandler>()`, a
  `SocketsHttpHandler { PooledConnectionLifetime = 2 min }` as primary handler
  and `SetHandlerLifetime(5 min)`. The first handler added is the outermost one.
- A timeout and a caller cancellation are both `OperationCanceledException`. The
  filter `when (!cancellationToken.IsCancellationRequested)` is what tells them
  apart: only the timeout becomes `GameServiceTimeoutException`.
- `GetGameAsync` uses `GetAsync` instead of `GetFromJsonAsync` because the latter
  throws on 404 and hides the status code. The response is disposed with `using`.
- `ReadFirstLineAsync` uses `HttpCompletionOption.ResponseHeadersRead`. With the
  default option `GetAsync` waits for the whole body, which the gated stream in the
  test never delivers, so the test would time out.
- Tests swap the primary handler with `ConfigurePrimaryHttpMessageHandler(() => fake)`
  registered after `AddGamesClient`; the last registration wins, and the rest of the
  pipeline (the `DelegatingHandler`) still runs. The pooled-lifetime test replays the
  builder actions on an `HttpMessageHandlerBuilder` to inspect the primary handler.

## Bonus (Optional)

- TODO-08 (optional): `StreamGamesAsync` returns `GetFromJsonAsAsyncEnumerable<Game>`,
  which yields items as they are parsed.
- TODO-09 (optional): the per-call overload links the caller's token with a
  `CancelAfter`, and reports expiry of the linked source as a timeout only when
  the caller's own token is not cancelled.

## Running the Lab

```bash
dotnet test
```
