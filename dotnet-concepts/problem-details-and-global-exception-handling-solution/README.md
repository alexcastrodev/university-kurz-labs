# ProblemDetails and Global Exception Handling (Solution)

Official solution for the `problem-details-and-global-exception-handling` lab.

## What changed from the starter

- `Program.cs`: `AddProblemDetails` with `CustomizeProblemDetails` setting
  `Instance`, `AddExceptionHandler<DomainExceptionHandler>()`, then
  `UseExceptionHandler()` and `UseStatusCodePages()`.
- `DomainExceptionHandler`: a `switch` maps exception types to a status and title,
  returns `false` for anything it does not own, and writes through
  `IProblemDetailsService` so the body shape matches the rest of the API.

## Why it is shaped this way

- `AddProblemDetails` only registers the writer. Unknown routes get a body when
  `UseStatusCodePages` runs, and exceptions get one when `UseExceptionHandler`
  runs.
- Unhandled exceptions (`/boom`) are not claimed by any handler, so the default
  generic `500` is written. The message `password=secret123` never reaches the
  client. The `Detail` is only filled for our own `GameNotFoundException`.
- `Retry-After` is set before writing, because the body write starts the response.

## Bonus notes

- The default writer only answers when the `Accept` header allows JSON.
- A handler that returns `true` without writing leaves an empty response.

## Running

```bash
dotnet test
```
