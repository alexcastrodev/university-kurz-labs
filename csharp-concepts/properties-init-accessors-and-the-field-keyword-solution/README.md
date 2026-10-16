# Properties, init Accessors and the field Keyword (Solution)

Official solution for the `properties-init-accessors-and-the-field-keyword` lab.

## What changed from the starter

- `field` is the compiler-generated backing field; declaring a body for one accessor keeps the other accessor auto-implemented.
- A property with a validated setter keeps its previous value when the setter throws, because the assignment to `field` happens after the check.
- `init` accessors run during object initializers and `with` expressions, so the same rule protects both paths.
- `get => field ??= "untitled"` initializes lazily on first read; the initializer `= null!` only silences nullable analysis for the declaration.
- `Counter` compares `field` with `value` before assigning, so assigning the current value stays silent.
- Bonus: `[field: ...]` targets the backing field; the field is named `<Token>k__BackingField`. If a class has its own member called `field`, write `@field` to refer to it.

## Running

```bash
dotnet test
```
