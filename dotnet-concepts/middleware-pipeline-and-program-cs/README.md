# The Middleware Pipeline and Program.cs

## Goal

Read `Program.cs` as a pipeline: know what `Use`, `Run` and `Map` do, why order
matters, how a short-circuit looks from outside, and how to write a
convention-based middleware.

## Prerequisites

- Basic minimal APIs
- `async`/`await`
- .NET 10 SDK

## Task

The app has a maintenance gate, a correlation id middleware and a few endpoints,
but the pipeline is incomplete and partly in the wrong order. Tests drive the
requests through the in-memory server and check headers, status codes and bodies.

## Instructions

Complete the following TODOs:

- TODO-00: Add an exception-catching middleware and put it where it can wrap
  everything else.
- TODO-01: Fix the order of the gate and the correlation middleware.
- TODO-02: Add the `/branch` pipeline with `Map` and `Run`, and observe how `Path`
  and `PathBase` change.
- TODO-03: Implement `CorrelationIdMiddleware.InvokeAsync`.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-04 (optional): Register a terminal `app.Run(...)` before the endpoints and see
  which tests break, then remove it.
- TODO-05 (optional): Add a request timer middleware that sets `X-Elapsed-Ms`
  using `Response.OnStarting` (why can it not set the header after `next`?).
