# Exceptions: try, catch, Filters and finally (Solution)

Official solution for the `exceptions-try-catch-filters-and-finally` lab.

## What changed from the starter

- Catch order matters: `FileNotFoundException` must come before `IOException`, otherwise the compiler reports CS0160. The filter `when (e.StatusCode >= 500)` does not catch the exception, so a non-matching `RemoteException` falls to the next clause.
- `ObserveTwoPasses` logs `filter`, `finally`, `catch`: the runtime first looks for a handler (running filters), and only then unwinds and runs `finally` blocks. A filter with side effects is therefore observable before cleanup, and a filter must never throw.
- `ExceptionDispatchInfo.Capture(e).Throw()` rethrows from another place and appends to the original stack trace; `throw e;` would reset it.
- `Task.WhenAll` faults with all the exceptions, but `await` only surfaces the first. `Wait()` surfaces the `AggregateException` with every inner exception.
- `OrderRejectedException` follows the standard shape: message, extra data, optional inner exception.

## Running

```bash
dotnet test
```
