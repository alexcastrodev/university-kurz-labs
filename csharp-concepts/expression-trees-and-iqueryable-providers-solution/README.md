# Expression Trees and IQueryable Providers (Solution)

Official solution for the `expression-trees-and-iqueryable-providers` lab, with the optional tasks implemented.

## What changed from the starter

- `Builders`: `TotalGreaterThan` is built from factory methods. `TotalAboveCaptured` takes the body of `() => min`, a member access on the closure object, so the tree holds the variable and not `150`.
- `ReplaceParameter` and `Combinators.Or`/`And`: the right body is rewritten to use the left parameter before `OrElse`/`AndAlso`. Without the rewrite the lambda would reference a parameter it does not declare, and `Compile()` throws.
- `MemberCollector`: overrides `VisitMember` and calls `base` so nested accesses are still visited.
- `LoggingProvider`: `CreateQuery` logs the operator and wraps the inner result; `Execute` logs the whole tree. `Count()` goes through `Execute`, enumeration through `GetEnumerator`.
- `OrderQueries.Filter`: chained `Where` calls compose with AND, and a null filter adds no node at all.
- `OrderQueries.OrderByProperty` (bonus): builds the `Queryable.OrderBy` call node and hands it to `Provider.CreateQuery`, which is what `Queryable.OrderBy` does itself.

## Why it is shaped this way

- A captured variable is a field on a closure object. EF Core turns it into a SQL parameter; an `Expression.Constant` per value produces a new query shape per value.
- `Compile()` is expensive; compile once and cache the delegate if you need to run a tree repeatedly.
- The compiler rejects constructs that have no node type: statement lambdas (CS0834), `?.` (CS8072), `is` patterns (CS8122) and switch expressions (CS8514).

## Running

```bash
dotnet test
```
