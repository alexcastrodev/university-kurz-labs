# Type, Constant and Relational Patterns (Solution)

Official solution for the `type-constant-and-relational-patterns` lab.

## What changed from the starter

- In `Describe`, order is part of the logic: `int and < 0` comes before the constant `0`, which comes before the general `int`. Swapping them makes later arms unreachable and the compiler says so (subsumption).
- `and` binds tighter than `or`, so `IsAsciiLetter` needs the parentheses to express two ranges.
- In `TicketPrice` the rows are tested top to bottom, so the membership check only needs to cover the ages not already handled.
- `Next` has no discard arm: the compiler warns (CS8524) because an enum can hold undefined values, and the call with `(Light)99` throws `SwitchExpressionException`. The solution suppresses the warning on purpose; in production code you usually prefer an explicit `_ => throw` with a better message.

## Running

```bash
dotnet test
```
