# Data-Driven Tests in MSTest: DataRow, DynamicData and Custom Sources (Solution)

Official solution for the `mstest-data-driven-tests` lab.

## What changed from the starter

- **`WeightBandTests`**: the colleague's loop checked 500 g, 3 kg, 12 kg and 25 kg. Changing `<=` to `<` in
  the band lookup left it green, because no value sat on an edge. Swapping two band prices turned it red but
  reported only the first wrong value, because the first failing `Assert` ends the test. The solution has one
  row per case, on both sides of every edge, so a failure names the exact weight. The expected prices are
  `decimal`, and C# does not allow a `decimal` as an attribute argument: `[DataRow(1_000, 5.00)]` passes a
  `double`, the build warns (MSTEST0014) and the row fails at run time because MSTest does not convert it.
  A `[DynamicData]` property of `(int Grams, decimal Expected)` tuples keeps the values typed.
- **`InvalidInputTests`** uses `[DataRow]` with `DisplayName` for the weights. Every check throws
  `ArgumentOutOfRangeException`, so the type alone cannot say which rule fired: `Quote(0, Zone.Domestic, -1m)`
  is rejected by the weight check, and a test written like that stays green with the order total check
  deleted. The solution passes a valid weight and asserts `ParamName`.
- **`PromoCodeTests`** shows `[DataRow(null)]` with a `string?` parameter, and near misses that must pay.
- **`ParcelTests`** puts the expected total in cents (an `int` fits in an attribute) and the parcels last, in
  a `params int[]`, so rows have different lengths. An array literal is also a valid attribute argument.
- **`ShippingCases`** holds `TestDataRow<(decimal, Zone, decimal)>` rows with a `DisplayName` each, and
  `FreeShippingTests` reads them with `[DynamicData(nameof(ShippingCases.Thresholds), typeof(ShippingCases))]`.
  The output shows "exactly the threshold ships free" instead of a list of values.
- **`BandEdgesAttribute`** implements `ITestDataSource`: it reads the edges from `ShippingCalculator.Bands`
  and yields one row per edge and zone, 9 in total. It takes only the inputs from the production code.
  `MonotonicPriceTests` asserts a property (one gram past an edge costs more), never a price computed from the
  same table, which would agree with any bug in it.
- **`ZoneMatrixTests`** (bonus) uses `[CombinatorialData]`: 3 zones times 4 weights is 12 cases. Two more
  parameters with three values each would make it 108.

## Bonus notes

- The solution runs 53 tests from 13 test methods: data rows are counted one by one.
- Mutation check, run against this solution. Each change below to `src/Shipping` was tried on its own:

| Change to the production code | Tests that fail |
| --- | --- |
| Band lookup uses `<` instead of `<=` | 4 edge rows of `ADomesticParcelIsPricedByItsWeightBand`, all 9 `[BandEdges]` rows, 3 combinatorial rows (30 000 g) |
| Free shipping uses `>` instead of `>=` | "exactly the threshold ships free" |
| Promo code compared with `Ordinal` | "the code is case-insensitive", "surrounding spaces are ignored" |
| Promo code not trimmed | "surrounding spaces are ignored" |
| Prices of the 5 kg and 20 kg bands swapped | 4 band rows, 3 `[BandEdges]` rows, 2 parcel rows |
| Maximum weight check deleted | "one gram over the limit", the `int.MaxValue` row, one `OneInvalidParcelRejectsTheWholeOrder` row |
| Free shipping applied to every zone | `TheCodeDoesNotWorkAbroad`, the neighbor and international threshold rows |
| Order total check deleted | `ANegativeOrderTotalIsRejected` |
| International multiplier set to 0 | 3 `[BandEdges]` rows, 4 combinatorial rows, the international threshold row |
| An order without parcels accepted | `AnOrderWithoutParcelsIsRejected` |

- The two traps were checked too: the starter's loop stays green with `<` in the band lookup, and
  `Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ShippingCalculator.Quote(0, Zone.Domestic, -1m))`
  stays green with the order total check deleted.
