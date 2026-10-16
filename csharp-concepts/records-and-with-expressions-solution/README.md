# Records and with Expressions (Solution)

Official solution for the `records-and-with-expressions` lab.

## What changed from the starter

- `customer with { Address = customer.Address with { City = city } }` creates two new objects; `with` copies only the reference of nested members, so the nested `with` is what keeps the original `Address` untouched.
- `[.. customer.Tags, tag]` allocates a new list. Reusing `customer.Tags` and calling `Add` would change both customers.
- `with` calls the `init` accessors, not the constructor. The explicit `Amount` property validates in both paths: the property initializer for the constructor and the accessor for `with`.
- Record equality compares members with their own `Equals`; a `List` or `IReadOnlyList` compares by reference, so equal content is not equal records. `SequenceEqual` fixes the comparison, or use an immutable collection with value semantics in your own type.

## Running

```bash
dotnet test
```
