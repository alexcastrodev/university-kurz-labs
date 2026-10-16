# Inheritance: virtual, override, new and sealed (Solution)

Official solution for the `inheritance-virtual-override-and-sealed` lab.

## What changed from the starter

- `Speak()` is picked by the runtime type through the base reference, while `Describe()` is hidden with `new` and picked by the static type of the variable.
- `Report` is `abstract` with a `protected` constructor and `SalesReport` is `sealed`.
- `Dog Clone()` is a covariant override (C# 9): callers get a `Dog` without a cast.
- The switch lists the more specific `Circle { Radius: > 10 }` first; `null` falls to `_`.
- `CountingBag` wraps a list, so `AddRange` of three items reports `Added == 3` instead of 6; `Bag` is kept only to show the original problem.
- Bonus: `sealed override` shows as `IsFinal` in reflection.

## Running

```bash
dotnet test
```
