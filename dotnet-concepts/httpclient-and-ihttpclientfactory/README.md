# HttpClient and IHttpClientFactory

## Goal

Register a typed client the way production code should (base address, outgoing
handler, pooled connection lifetime, handler rotation) and handle the three ways
a call can end early: a 404, a timeout and a cancellation. No network is used:
the tests replace the primary handler with an in-process fake.

## Prerequisites

- Basic `async`/`await` and `CancellationToken`
- Dependency injection (service lifetimes)
- .NET 10 SDK

## Task

`AddGamesClient` registers a bare typed client, `CorrelationIdHandler` passes
requests through untouched and `GamesClient` throws `NotImplementedException`.
You will configure the factory, write the outgoing middleware and implement the
client calls.

## Instructions

Complete the following TODOs:

- TODO-00: Set `BaseAddress` and `Timeout` on the typed client.
- TODO-01: Register and add `CorrelationIdHandler`.
- TODO-02: Make the primary handler a `SocketsHttpHandler` with a pooled connection lifetime.
- TODO-03: Set the factory handler lifetime.
- TODO-04: Add `X-Correlation-Id` unless the request already has one.
- TODO-05: `GetGameAsync`: `null` for 404, exception for other failures, JSON otherwise.
- TODO-06: Translate `HttpClient.Timeout` into `GameServiceTimeoutException` without hiding the caller's cancellation.
- TODO-07: `ReadFirstLineAsync` must return before the body has finished arriving.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): `StreamGamesAsync` with `GetFromJsonAsAsyncEnumerable`.
- TODO-09 (optional): A per-call timeout built with a linked `CancellationTokenSource`.
