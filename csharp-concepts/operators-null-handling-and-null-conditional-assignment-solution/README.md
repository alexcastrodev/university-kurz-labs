# Operators, Null Handling and Null-Conditional Assignment (Solution)

Official solution for the `operators-null-handling-and-null-conditional-assignment` lab.

## What changed from the starter

- `customer?.Name = newName()` means `if (customer is not null) customer.Name = newName();` with `customer` read once. The right side never runs for a null receiver.
- The same rule covers compound assignment (`?.Age += 1`), `??=`, `+=` on events and indexers (`map?[key] = ...`). Used as a value, the result is the nullable version of the assigned type: `int?` for `Birthday`.
- It does not cover `++`/`--` (CS1059), `ref`/`out` arguments (CS1510), deconstruction targets (CS0131) or struct receivers.
- `numbers?.Length < 2` is `false` for null because every comparison with a null `int?` is false. `(numbers?.Length ?? 0) < 2` says what null means.
- `bonus ?? 0 + 10` is `bonus ?? (0 + 10)`: parenthesize the `??`.
- `first() & second()` always evaluates both operands; `&&` stops after a `false` first operand.
- Bonus: `getCustomer()?.Age += 10` calls `getCustomer` once even though a compound assignment reads and writes the property, and `TryRename` shows that the value of `a?.b = x` is `null` when nothing was written.

## Running

```bash
dotnet test
```
