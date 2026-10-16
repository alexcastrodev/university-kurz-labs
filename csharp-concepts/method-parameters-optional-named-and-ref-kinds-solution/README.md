# Method Parameters: Optional, Named and ref Kinds (Solution)

Official solution for the `method-parameters-optional-named-and-ref-kinds` lab.

## What changed from the starter

- `(first, second) = (second, first)` swaps through the `ref` aliases without a temporary.
- `Math.DivRem` returns a tuple that deconstructs straight into the `out` parameters.
- `ref readonly` returns an alias of the array element; the caller can read it without copying the struct but cannot assign through it.
- `params ReadOnlySpan<int>` accepts `Sum(1, 2, 3)`, `Sum()` and a collection expression, without allocating an array for the call.
- Optional values are baked into the call site at compile time: changing a default in a library does not affect already compiled callers.
- `Pick(5)` binds to `long`: `int` to `long` is a better conversion than `int` to `double` or boxing to `object`.
- Bonus: a `ref` return makes `FindSlot(values, 2) = 20` write into the array; `int.TryParse(text, out _)` discards the value.

## Running

```bash
dotnet test
```
