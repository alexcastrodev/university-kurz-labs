# Numeric Conversions, Rounding and Overflow

## Goal

Choose the conversion, rounding mode and overflow policy explicitly instead of trusting the default of a cast, `Convert` or `Math.Round`.

## Prerequisites

- C# numeric types
- `checked` and `unchecked`
- Exceptions
- a .NET 10 SDK

## Task

`Conversions` is stubbed. Each method has one edge case where the obvious code gives a wrong or silent result (NaN, halves, wrap-around).

## Instructions

Complete the following TODOs:

- TODO-00: `ToPixel`: round half away from zero, clamp to 0..255, NaN to 0.
- TODO-01: `RoundToCents`: pick `MidpointRounding` explicitly.
- TODO-02: `ToIndex`: truncate, and fail loudly with a checked cast.
- TODO-03: `ToIntWithConvert`: `Convert.ToInt32` rounds halves to even, unlike the cast.
- TODO-04: `SumExact`: detect wrap-around with `checked`.
- TODO-05: `TryAddExact`: the Try pattern, without exceptions.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `NarrowChecked<TFrom, TTo>` using `TTo.CreateChecked`.
- TODO-07 (optional): `AddSaturating`: clamp to the int range instead of throwing or wrapping.
- TODO-08 (optional): `SplitEvenly`: split money into parts that add up to the exact total.
