# The Middleware Pipeline and Program.cs (Solution)

Official solution for the `middleware-pipeline-and-program-cs` lab.

## What changed from the starter

- `Program.cs`: the exception catcher is registered first, then
  `CorrelationIdMiddleware`, then the maintenance gate, then the `/branch` map.
- `CorrelationIdMiddleware`: reads or generates the id, stores it in `HttpContext.Items`
  and writes the response header before calling `next`.

## Why this order

- The catcher is outermost because it only sees exceptions from what comes after it.
- The correlation middleware runs before the gate. A short-circuit does not call `next`,
  so anything registered after the gate never runs for that request.
- `Map("/branch", ...)` moves the matched prefix from `Path` to `PathBase`, which is why
  the branch sees `pathBase=/branch path=/x` for `/branch/x`.
- Endpoints sit at the end of the pipeline (added by `WebApplication`), so a terminal
  `Run` registered earlier would swallow every request.

## Running

```bash
dotnet test
```
