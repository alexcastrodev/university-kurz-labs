# ProblemDetails and Global Exception Handling

## Goal

Make a `404`, an unknown route, a timeout and an unhandled exception all answer
with the same `application/problem+json` shape, without leaking exception
messages to clients.

## Prerequisites

- Basic minimal APIs
- Middleware order (see the pipeline lab)
- .NET 10 SDK

## Task

The API has four failing endpoints. Today they return empty bodies, or (for an
unhandled exception) whatever the host decides. You will wire the problem details
service, the exception handler middleware, status code pages and an
`IExceptionHandler` that maps your own exception types.

## Instructions

Complete the following TODOs:

- TODO-00: Register `AddProblemDetails` in `Program.cs` and set `Instance` to the
  request path for every document.
- TODO-01: Register `DomainExceptionHandler`.
- TODO-02: Add the exception handler middleware.
- TODO-03: Add status code pages. Registering the service alone does not write a
  body for an unknown route, find out which middleware asks for it.
- TODO-04: Map `GameNotFoundException` to `404` in `DomainExceptionHandler`.
- TODO-05: Map `TimeoutException` to `503`.
- TODO-06: Send a `Retry-After: 5` header for the timeout.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Request `/boom` with `Accept: text/html` using `curl` and
  compare it with `Accept: application/json`. What does the default writer do?
- TODO-08 (optional): Remove `return false` for unknown exceptions and return
  `true` without writing. What does the client receive?
