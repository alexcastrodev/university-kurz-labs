# LINQ: Sets, Joins, Grouping and Lookups (Solution)

Official solution for the `linq-sets-joins-grouping-and-lookups` lab.

## What changed from the starter

- `Join` and `GroupJoin` never match `null` keys, so the order with no customer disappears from the inner join and from the counts. The `(int?)c.Id` cast makes both key selectors the same type.
- `LeftJoin` passes a null inner element when there is no match, so the selector uses `o?.Total`. `GroupJoin` keeps every left element with a possibly empty group.
- `ToLookup` runs immediately: later changes to the source are not visible, and `lookup[missing]` is an empty sequence. `GroupBy` is deferred and re-evaluates the source on each enumeration.
- `ExceptBy` is a set operation: it returns distinct elements, so duplicate emails in the incoming list collapse without extra code.
- `TotalPerCustomer` ends with `ToList()` so the query does not run again (and does not see later changes) when enumerated twice.

## Running

```bash
dotnet test
```
