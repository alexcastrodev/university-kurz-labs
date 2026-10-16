# Generated Values, Defaults and Computed Columns (Solution)

## Goal

Declare what the database generates. See the starter project for the exercise statement
and the SQLite note about sequences and row versions.

## Solution notes

- `HasDefaultValue("Active")` works because `Status` is `string?`: `null` is the CLR default,
  EF Core's "not set" sentinel, so the column is left out of the `INSERT` and read back.
- For `Credits` the CLR default `0` would also mean "not set", swallowing an intentional zero.
  The initializer `= -1` plus `HasSentinel(-1)` makes `-1` mean "use the default" and `0`
  an ordinary value.
- `HasDefaultValueSql("CURRENT_TIMESTAMP")` is raw SQL, so it is provider specific
  (`GETUTCDATE()` on SQL Server).
- `HasComputedColumnSql(..., stored: true)` makes the property read-only for EF Core: it is
  never sent, and is read back after inserts and updates.
- The trigger changes `UpdatedAt` after the `UPDATE`. SQLite's `RETURNING` reports values before
  triggers run, so EF Core would read `null`. `ToTable(t => t.UseSqlReturningClause(false))`
  makes EF Core issue a follow-up `SELECT` instead. Removing that line fails
  `ShouldReadBackUpdatedAtSetByTheTrigger`.
- `ShouldApplyDefaultsToRowsInsertedOutsideEfCore` proves the defaults and the computed column
  live in the schema, not only in the model.

## Bonus (Optional)

- TODO-05 (optional): `HasValueGenerator<GuidValueGenerator>()` generates `Ref` before insert.
  Marking a non-key `Guid` generated without any generator fails or stores an empty Guid.
- TODO-06 (optional): `HasComputedColumnSql(..., stored: false)` is evaluated on read and
  occupies no storage.

## Running the Lab

```bash
dotnet test
```
