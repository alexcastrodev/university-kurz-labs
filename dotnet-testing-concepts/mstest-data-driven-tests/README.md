# Data-Driven Tests in MSTest: DataRow, DynamicData and Custom Sources

## Goal

A shipping calculator is already fully implemented: weight bands with edges at 1, 5, 20 and 30 kg, three
zones, free domestic shipping from an order of 100.00 or with a promo code, and orders with several parcels.
Your job is to write MSTest data-driven tests that prove its rules, and to learn where the bugs of a price
table hide: on the edges, not in the middle of the bands.

## Prerequisites

- MSTest 4 Fundamentals: Lifecycle, TestContext and Parallelism (the concept and its lab)
- .NET 10 SDK

## Task

Read `src/Shipping/ShippingCalculator.cs` first. Then open `tests/Shipping.Tests`. Every test method is
already declared with a hint, and fails with a `TODO-NN` message until you write it. Most of them have no
parameters yet: adding the parameters and the data rows is part of the exercise.

`WeightBandTests` already has a test that a colleague wrote, a loop over four weights. It passes, and it
would keep passing with a real bug in the band lookup.

## Instructions

Complete the following TODOs in `tests/Shipping.Tests`:

- TODO-00: `WeightBandTests`: break the band lookup and watch the loop stay green, then replace it with
  `[DynamicData]` rows on both sides of every band edge.
- TODO-01: `InvalidInputTests`: `[DataRow]` rows with `DisplayName`, and an assertion that knows which rule
  rejected the call.
- TODO-02: `PromoCodeTests`: `[DataRow]` with strings, `null` and near misses.
- TODO-03: `ParcelTests`: `params` arrays and array arguments in `[DataRow]`.
- TODO-04: `ShippingCases` and `FreeShippingTests`: `TestDataRow<T>` rows with display names, read by
  `[DynamicData]` from another class.
- TODO-05: `BandEdgesAttribute` and `MonotonicPriceTests`: a custom `ITestDataSource` that generates the
  edges from the band table, and a test that asserts a property instead of a price.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

To see the name of every data row:

```bash
dotnet test --logger "console;verbosity=normal"
```

## Bonus (Optional)

- TODO-06 (optional): Write `ZoneMatrixTests` with `[CombinatorialData]` (namespace
  `Microsoft.VisualStudio.TestTools.UnitTesting.Combinatorial`, MSTest 4.4 and later): a `Zone` parameter
  (all enum values are inferred) times `[CombinatorialValues(1, 1_000, 1_001, 30_000)] int grams`, asserting
  that every quote below the threshold is positive. Count the generated cases, then imagine adding the order
  total and the promo code as parameters.
- TODO-07 (optional): Mutation check. Change the production code on purpose, one change at a time: `<=` to
  `<` in the band lookup, `>=` to `>` in the free shipping threshold, `OrdinalIgnoreCase` to `Ordinal` in the
  promo code, and delete the `ThrowIfNegative(orderTotal)` check. Each change should turn at least one of
  your tests red. A change that leaves everything green means a missing test.
