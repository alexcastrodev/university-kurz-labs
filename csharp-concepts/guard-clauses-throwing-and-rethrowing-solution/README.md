# Guard Clauses, Throwing and Rethrowing (Solution)

Official solution for the `guard-clauses-throwing-and-rethrowing` lab.

## What changed from the starter

- The `ThrowIf` helpers replace four-line guards. They do not ask for the parameter name: the message and `ParamName` hold the whole expression (`order.Customer.Email`) thanks to `[CallerArgumentExpression]`, which `NotDefault` reuses for your own guards. `[NotNull]` tells the nullable analysis the value is not null after the call.
- `args.Length > 0 ? args[0] : throw ...` keeps a simple guard inside one expression.
- `Get` only tests the bounds; the heavy `throw new X($"...")` lives in `ThrowIndex`, marked `[DoesNotReturn]`. The caller shrinks (the BCL does this in its `ThrowHelper` classes), which matters on hot paths; in an ordinary service method a plain `throw` is fine.
- An `async` method puts every exception, even the first line's, in the returned `Task`. `SaveAsync` is a normal method that validates and returns the task of a local async function, so the caller fails at the call.
- `throw;` rethrows with the original stack trace; `throw ex;` restarts it at the current method (analyzer CA2200). Wrapping is right when a layer adds meaning, as long as the original goes in as `innerException`.
- Bonus: `ExceptionDispatchInfo.Capture(ex).Throw()` rethrows outside the `catch` and preserves the original trace, and `ObjectDisposedException.ThrowIf` is the guard for disposed objects (the `ObjectName` is the type's full name).

## Running

```bash
dotnet test
```
