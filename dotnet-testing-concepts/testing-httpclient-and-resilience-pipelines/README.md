# Testing HttpClient Code and Resilience Pipelines

## Goal

`FreightClient` is already fully implemented: a typed `HttpClient` for a freight API (quotes and shipment
manifests) behind an `IdempotencyKeyHandler` and a resilience pipeline from
`Microsoft.Extensions.Http.Resilience` (retry, circuit breaker, attempt timeout). Your job is to write the
MSTest tests that prove its eight rules, with a hand-written fake `HttpMessageHandler`, no mocking library,
no server, and a fake clock, so that a suite full of retries and timeouts runs in a fraction of a second.

## Prerequisites

- HttpClient and IHttpClientFactory, and Resilience: Retries, Timeouts and Circuit Breakers (dotnet-concepts)
- Controlling Time with TimeProvider, and Test Doubles: Fakes, Stubs and NSubstitute (this track)
- Basic MSTest: `[TestClass]`, `[TestMethod]`, `[DataRow]`, `TestContext`
- .NET 10 SDK

## Task

Read `src/Freight` first. `FreightClient.cs` lists the eight rules at the top, and
`FreightServiceCollectionExtensions.cs` shows the handler order and the pipeline options. Then open the
test project. Every test is declared with a hint and fails with a `TODO-NN` message until you write it.
`FakeFreightApi` is a shell for now.

Four traps are waiting, and each one makes a naive test pass for the wrong reason:

- **The fake that always answers 200.** A retry test that checks only the final result is green with the
  retry deleted. Script the answers, fail on unscripted requests, and count the attempts.
- **The retry that did not resend what you think.** A retry sends the same `HttpRequestMessage` instance
  again. A fake that stores the messages and reads their headers after the call sees the last attempt's
  headers three times, so "every attempt had the same Idempotency-Key" is green even when each attempt got
  a new one. Snapshot each attempt inside `SendAsync`.
- **The body you never read.** A manifest read from a forward-only stream cannot be sent twice unless the
  client buffers it. A fake that never reads the body, or reads it with `ReadAsStringAsync` (which
  buffers it for you), hides the bug. Read it with `CopyToAsync`, like a transport.
- **The timeout test that waits for real.** The pipeline waits on the fake clock, so a call you simply
  `await` sits there until `HttpClient.Timeout` (100 real seconds). Run every call through `RunAsync`, which
  advances the fake clock until the call completes.

## Instructions

Complete the following TODOs in `tests/Freight.Tests`:

- TODO-00: `FakeFreightApi`: scripted answers, `Hang()`, a snapshot per attempt, unscripted requests fail.
- TODO-01: `IdempotencyKeyHandlerTests`: the handler alone, through an `HttpMessageInvoker`.
- TODO-02: `FreightClientTests` constructor: a `ServiceCollection` with the fake clock, `AddFreightClient`,
  and `ConfigurePrimaryHttpMessageHandler` to replace only the last hop.
- TODO-03: a quote from a 200, and 4xx answers that are rejected without a retry.
- TODO-04: retries until success, retries that run out, the same key and body on every attempt.
- TODO-05: an attempt timeout on the fake clock, and a caller's cancellation that is not a timeout.
- TODO-06: the circuit opens and calls stop reaching the API.
- TODO-07: a malformed quote is a contract bug; a manifest from a forward-only stream survives a retry.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): after the break duration, the circuit lets a probe through and a 200 closes it.
- TODO-09 (optional): mutation check. Change the production code on purpose, one change at a time, and see
  which of your tests turn red: make the retry handle nothing (`ShouldHandle = _ => ValueTask.FromResult(false)`);
  register `IdempotencyKeyHandler` after `AddResilienceHandler`; remove the `AddTimeout`; remove the
  circuit breaker; remove `LoadIntoBufferAsync`; map every failure status to `Unavailable`; add a
  `catch (OperationCanceledException)` that returns `Unavailable("timeout")`; add a `catch (JsonException)`
  that returns `Unavailable`. A change that leaves everything green is a missing test.
