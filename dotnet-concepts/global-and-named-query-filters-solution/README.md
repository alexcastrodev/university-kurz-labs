# Global and Named Query Filters (Solution)

Official solution for the `global-and-named-query-filters` lab.

## What changed from the starter

- `AppDbContext`: `HasQueryFilter(TenantFilter, ...)` on `Blog`, and a loop over the
  `ISoftDelete` entity types that calls `SetQueryFilter(SoftDeletionFilter, lambda)`.
- `BlogQueries`: `IgnoreQueryFilters([name])` with only the name that should go away.

## Notes

- The filter reads `_tenantId`, a field of the context. EF Core turns it into a query
  parameter, so one cached model serves every tenant. The shape of the model (which filters
  exist) cannot depend on a constructor argument, only the values can.
- Before EF Core 10 a second `HasQueryFilter` replaced the first. With names, each rule is
  separate and can be ignored on its own.
- `IgnoreQueryFilters(["Typo"])` is accepted silently, so the names live in constants.
- An unnamed filter cannot be mixed with named ones on the same entity.
- `Undelete` keeps the tenant filter on, so a blog from another tenant is `null`.

## Running

```bash
dotnet test
```
