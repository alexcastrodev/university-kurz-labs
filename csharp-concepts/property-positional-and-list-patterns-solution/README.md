# Property, Positional and List Patterns (Solution)

Official solution for the `property-positional-and-list-patterns` lab.

## What changed from the starter

- `{ Address.City: "Lisbon", Orders: > 0 }` is null-safe at every step: a null `Address` simply does not match.
- A positional pattern calls `Deconstruct` (free on positional records) once per `switch`, not once per arm. Arm order is the logic: the specific `(0, 0)` comes before `(_, 0)`.
- A tuple `switch` over `(state, trigger)` makes every transition one line, and the discard arm documents what is invalid.
- `[var first, .. var rest]` needs a countable and indexable input, and capturing the slice needs `Range` or `Slice` support. For `int[]` and `string` the slice is a copy (`GetSubArray`, `Substring`).
- `["move", var x, var y] when ...` must come before `[var command, ..]`, otherwise the catch-all would shadow it, and a failed `when` falls through to the next arm.
- `value is (0, 0)` on an `object` works through `ITuple` for a boxed `ValueTuple`; a record needs the type pattern `Point(0, 0)`.
- Bonus: over a `ReadOnlySpan<int>` the slice is a view, so the recursive `Sum` allocates nothing (the same recursion over `int[]` copies on every call, a quadratic trap). Any type with a `Deconstruct` method, like `Rgb`, can be matched positionally.

## Running

```bash
dotnet test
```
