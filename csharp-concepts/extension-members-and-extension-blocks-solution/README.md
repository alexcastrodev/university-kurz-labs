# Extension Members and Extension Blocks (C# 14) (Solution)

Official solution for the `extension-members-and-extension-blocks` lab.

## What changed from the starter

- A block is `extension<T>(IEnumerable<T> source) { ... }`; the type parameter lives on the block because the receiver uses it.
- An extension property such as `IsEmpty` is emitted as a static `get_IsEmpty` method, so `SequenceExtensions.JoinWith(names, ",")` also works.
- `extension(IEnumerable<int>)` without a receiver name can only hold static members.
- A `null` receiver does not throw by itself, and `extension(ref int value)` mutates the caller variable (a `ref` receiver must be a value type).
- Bonus: operators and static methods on `Math` are members of extension blocks too; there are no extension fields or auto-properties.

## Running

```bash
dotnet test
```
