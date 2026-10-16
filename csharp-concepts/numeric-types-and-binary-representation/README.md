# Numeric Types and Binary Representation

## Goal

See what numbers really are in memory: two's complement integers, shifts and masks, IEEE 754 doubles that need a tolerance, and `decimal` with its scale for money.

## Prerequisites

- C# basic types and operators
- Formatting values with an explicit culture
- .NET 10 SDK

## Task

`Bits` and `Numbers` are stubbed. Each TODO targets one representation, and the tests pin the surprising behavior (`int.MinValue`, `1 << 32`, `0.1 + 0.2`, `1.0m` versus `1.00m`).

## Instructions

Complete the following TODOs:

- TODO-00: `ToBinary32` and `Negate`: two's complement bits, and negation as invert plus one (with the `int.MinValue` edge).
- TODO-01: `ShiftRightArithmetic`, `ShiftRightLogical` (`>>>`) and `OneShiftedLeft`: sign copy, zero fill and the masked shift count.
- TODO-02: `PackRgb`, `Channel`, `SetFlag`, `ClearFlag`, `HasFlag`: pack fields and flags into a `uint` with shifts and masks.
- TODO-03: `CountSetBits` and `NextPowerOfTwo`: use `BitOperations` instead of loops.
- TODO-04: `Near`: compare doubles with a combined absolute and relative tolerance, never `==` and never `double.Epsilon`.
- TODO-05: `Scale` and `ToCents`: read the decimal scale and convert prices to exact cents, rejecting fractions of a cent.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Write `MeanOfFinite(double[] values)` that ignores `NaN` and infinities (use `double.IsFinite`) and throws when nothing is finite. Add your own tests, including a sort of a list that contains `NaN`.
- TODO-07 (optional): Multiply two `long.MaxValue` values with `Int128` and compare with `unchecked` `long` multiplication. Which one wraps?
