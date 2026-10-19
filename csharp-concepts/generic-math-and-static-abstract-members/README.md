# Generic Math and static abstract Interface Members

## Goal

Write one algorithm that works for `int`, `double`, `decimal`, `BigInteger` and your own value type, by constraining the type parameter to the smallest `System.Numerics` interfaces it needs, and see where integer division and `checked` change the result.

## Prerequisites

- C# generics and constraints
- Interfaces with `static abstract` members (C# 11)
- .NET 10 SDK

## Task

`Algorithms` holds generic methods constrained by generic math interfaces. `Money` is a value type that is not a number: it has no division and no comparison, only `+` and a zero. `SafeMoney` is the same idea with a `checked` operator.

## Instructions

Complete the following TODOs:

- TODO-00: `Sum`: use `T.AdditiveIdentity` and `+=` through the smallest possible constraint.
- TODO-01: `Money`: implement `operator +` and `AdditiveIdentity` so your own type joins `Sum`.
- TODO-02: `Average` and `AverageAs`: `T.CreateChecked` for the count, and a second type parameter to avoid integer division and overflow.
- TODO-03: `Clamp01`: `T.Clamp` with `T.Zero` and `T.One` for any `INumber<T>`.
- TODO-04: `Bits`: `T.PopCount` for every integer width.
- TODO-05: `SumChecked` and `SafeMoney`: ask for `checked` in the algorithm and let the type honor it with `operator checked`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Try `List<INumber<int>> list = [];` and `var z = INumber<int>.Zero;`. Read the compiler errors (CS8920, CS8926) and explain why these interfaces are constraints, not types for variables.
- TODO-07 (optional): `Clamp01` uses only `T.Clamp`, `T.Zero` and `T.One`, yet it demands the whole `INumber<T>`. Which smaller interfaces would still give you zero, one and comparisons? What would you gain for callers such as `Money`, and what do you lose in readability?
