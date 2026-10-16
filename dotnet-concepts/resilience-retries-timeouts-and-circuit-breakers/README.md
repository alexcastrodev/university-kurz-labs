# Resilience: Retries, Timeouts and Circuit Breakers

## Goal

Configure `Microsoft.Extensions.Http.Resilience` (Polly v8) on named clients and
observe, with a handler that fails on a script, what each strategy does to the
number of requests that reach the server.

## Prerequisites

- `IHttpClientFactory` and named clients (see the HttpClient lab)
- Idempotency of HTTP methods
- .NET 10 SDK

## Task

Two clients exist without any resilience: `orders` (retries and timeouts) and
`catalog` (circuit breaker). The tests use `ScriptedHandler`, an in-process
`HttpMessageHandler` that counts hits and answers 503, hangs or recovers on
demand, so there is no network and no real server.

## Instructions

Complete the following TODOs:

- TODO-00: `orders` uses the standard resilience handler configured by `ConfigureOrders`.
- TODO-01: `catalog` uses the standard resilience handler configured by `ConfigureCatalog`.
- TODO-02: Retries: 2 attempts, 10 ms delay.
- TODO-03: Do not retry unsafe methods (a retried POST can create three orders).
- TODO-04: Attempt timeout 300 ms, total timeout 5 s, sampling duration 10 s.
- TODO-05: Turn retries off for GET on `catalog`.
- TODO-06: Circuit breaker: minimum throughput 5, failure ratio 0.5, sampling 10 s, break 1 s, attempt timeout 1 s.

Read the validation errors the options throw: they are part of the lesson.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): `pricing` with `AddStandardHedgingHandler` (delay 100 ms, one hedged attempt).
- TODO-08 (optional): `strict` with a custom `AddResilienceHandler` pipeline: timeout 1 s, retry, timeout 300 ms.
