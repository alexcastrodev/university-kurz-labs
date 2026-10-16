# EF Core Model Building: Conventions, Annotations and Fluent API (Solution)

## Goal

Configure the model with the right tool for each rule. See the starter project for
the exercise statement.

## Solution notes

- Precedence is Fluent API over data annotation over convention. `Name` has
  `[MaxLength(50)]` and the Fluent `HasMaxLength(120)` wins.
- `DefaultStringLengthConvention` is an `IModelFinalizingConvention` that uses
  `property.Builder.HasMaxLength`. The convention builder never overrides a stronger
  source, so `Category.Slug` keeps its attribute value of 50 while `Category.Name` and
  `Product.Note` get 200. `Product.Name` stays 120.
- `Properties<ProductStatus>().HaveConversion<string>()` is pre-convention configuration:
  right for a rule that must hold everywhere.
- `ApplyConfigurationsFromAssembly` finds both `IEntityTypeConfiguration<T>` classes, so
  adding an entity configuration never touches the context.
- `HasData` needs explicit keys and is part of the migration; `UseSeeding` runs code at
  `EnsureCreated`/`Migrate` time and does not appear in the migration.
- Tests read the design-time model (`IDesignTimeModel`) because relational facets such as
  the column name exist only there. SQLite stores `decimal` as `TEXT`, so precision is
  metadata the database does not enforce, while the unique index and the check
  constraint are enforced.

## Bonus (Optional)

- TODO-08 (optional): `UseSeeding` adds `Added at runtime` (id 3) if it is missing.
- TODO-09 (optional): `ToTable("products", t => t.HasCheckConstraint(...))`.
- TODO-10 (optional): `PreConventionShopDbContext` sets the default with
  `HaveMaxLength(200)`. Pre-convention configuration counts as explicit, so `Slug`
  becomes 200 and the `[MaxLength(50)]` attribute loses. It needs its own `DbContext`
  subclass because EF Core caches one model per context type.

## Running the Lab

```bash
dotnet test
```
