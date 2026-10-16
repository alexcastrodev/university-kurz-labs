# Resilience: Retries, Timeouts and Circuit Breakers (Solution)

## Goal

Configure the standard resilience pipeline on named clients and read its behavior
from the number of requests that reach a scripted in-process handler.

## Solution notes

- `AddStandardResilienceHandler` returns a pipeline builder, not an `IHttpClientBuilder`,
  so the code keeps the builder from `AddHttpClient` in a variable and returns that one.
  Tests call `ConfigurePrimaryHttpMessageHandler` on it to swap in the fake.
- A POST that fails once reaches the handler once because of
  `Retry.DisableForUnsafeHttpMethods()`. By default every method is retried.
- A timed-out attempt surfaces as Polly's `TimeoutRejectedException` (not
  `TimeoutException`) after 1 + 2 retries, so the handler sees 3 hits.
- `MaxRetryAttempts` must be at least 1, so retries on `catalog` are turned off with
  `Retry.DisableFor(HttpMethod.Get)`. Without that, one failing GET counts 3 attempts
  and the breaker arithmetic in the test changes.
- The options validator requires `SamplingDuration >= 2 * AttemptTimeout`. With the
  default 10 s attempt timeout and 30 s sampling it passes; as soon as you shorten the
  sampling window you must shorten the attempt timeout too (or vice versa).
- With `MinimumThroughput = 5` the breaker opens after five 503s; calls six to eight
  throw `BrokenCircuitException` and the handler count stays at 5. After the 1 s
  break the next call is a probe, and its success closes the circuit. The default
  minimum throughput of 100 would never trip in a test.
- Tests that hang the handler wrap the call in `WaitAsync` so a missing timeout
  fails the test instead of blocking it.

## Bonus (Optional)

- TODO-07 (optional): `AddStandardHedgingHandler().Configure(...)` sets
  `Hedging.Delay` to 100 ms. The first request sleeps 3 s, the hedged one answers
  at once, so the call returns in about 100 ms with two hits. Use hedging only for
  safe, idempotent reads, it doubles load on a slow dependency.
- TODO-08 (optional): strategies run in the order added. The outer 1 s timeout caps the
  whole retry sequence (it throws with `Timeout == 1 s`), the inner 300 ms one bounds each
  attempt, so only 2 or 3 attempts fit.

## Running the Lab

```bash
dotnet test
```
