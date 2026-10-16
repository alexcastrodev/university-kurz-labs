# Parameter Binding and Endpoint Filters (Solution)

Official solution for the `parameter-binding-and-endpoint-filters` lab.

## What changed from the starter

- `Program.cs`: `AddSingleton<Clock>()`, `[FromServices]` on the `/time` parameter
  and `AddEndpointFilter<OrderValidationFilter>()` on the `/orders` group.
- `Point.TryParse`: splits on a comma and parses both halves with
  `CultureInfo.InvariantCulture`.
- `Caller.BindAsync`: header, then query string, `null` when absent.
- `SearchRequest`: `[FromRoute]`, `[FromQuery]` and `[FromHeader(Name = "X-Tenant")]`.
- `OrderValidationFilter`: validates, returns `Results.ValidationProblem`, and
  replaces `ctx.Arguments[0]` with a trimmed copy before calling `next`.

## Why every endpoint failed

`Clock` is a class, and on a `GET` the framework treats an unregistered class as an
inferred body, which is not allowed. The endpoints are built together on the first
request, so one bad signature makes every request fail with `500`. With
`[FromServices]` the failure is local and says what is missing: no service for type
`Clock` has been registered.

## Notes

- `BindAsync` returning `null` is a `400`, throwing is a `500`.
- A filter can change what the handler receives by assigning `ctx.Arguments[i]`.

## Running

```bash
dotnet test
```
