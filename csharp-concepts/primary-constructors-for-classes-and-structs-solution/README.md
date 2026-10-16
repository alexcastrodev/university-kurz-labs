# Primary Constructors for Classes and Structs (Solution)

Official solution for the `primary-constructors-for-classes-and-structs` lab.

## What changed from the starter

- `Account` keeps one source of truth, the `Owner` property; `Describe()` and `Rename()` no longer touch the parameter, so no hidden `<owner>P` field is created.
- `OrderService` validates in a field initializer (there is no constructor body); a purely captured parameter would only fail at first use.
- A struct with a primary constructor still has a zeroed `default`; an explicit parameterless constructor must chain with `this(...)`.
- `Savings` reads the inherited `Id` and `Owner`; the solution csproj sets `WarningsAsErrors` for CS9124 and CS9107.
- Bonus: a record parameter list generates `init` properties and value equality; a class one does not. A same-named `readonly` field shadows the parameter inside members.

## Running

```bash
dotnet test
```
