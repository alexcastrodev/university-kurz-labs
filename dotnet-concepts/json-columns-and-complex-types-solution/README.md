# JSON Columns and Complex Types (Solution)

## Goal

Map complex types to JSON, query and update inside them. See the starter project for
the exercise statement.

## Solution notes

- `ComplexProperty(...).ToJson()` and `ComplexCollection(...).ToJson()` are the EF Core 10
  way. A complex collection must be mapped to JSON. `Tags` is a collection of primitives,
  mapped to a JSON array by convention since EF Core 8.
- On SQLite the filters translate to `->>` and `json_each`; no code in the queries
  mentions JSON.
- `SaveChanges` rewrites the whole JSON column even for one changed member. `Address` is a
  record, so `with` produces the replacement value.
- `ExecuteUpdate` with `SetProperty(c => c.Address.City, ...)` becomes a partial
  `json_set` update that never loads the entities. It does not work for owned types.
- The database does not check the shape of a document. `SetProperty(c => c.Shipping[0].City, ...)`
  on a row with an empty array makes SQLite create `[{"City":"X"}]`, which EF Core later
  loads with null `Street` and `Country`. Hence the `Where(c => c.Shipping.Count > 0)` guard.
- The tests use `Database.SqlQuery<string>` with `json_extract` to prove that the data lives
  in one JSON column, not in `Address_City` style columns.
- SQLite caveats from the concept: `SelectMany` over a JSON collection is not supported
  there, and comparing whole JSON values compares text, so neither is part of the lab.

## Bonus (Optional)

- TODO-08 (optional): `SetFirstShippingCityAsync` with the `Count > 0` guard.
- TODO-09 (optional): `StoredAddressJsonAsync` with `SqlQuery<string>`.

## Running the Lab

```bash
dotnet test
```
