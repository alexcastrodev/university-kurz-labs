# EF Core Model Building: Conventions, Annotations and Fluent API

## Goal

See who wins when a convention, a data annotation and the Fluent API disagree, and
organize the configuration with `IEntityTypeConfiguration<T>`, a model finalizing
convention and `HasData`. Tests inspect the EF Core model metadata and run against
SQLite in memory, so no database server is needed.

## Prerequisites

- EF Core basics (`DbContext`, `DbSet`, migrations idea)
- Attributes such as `[MaxLength]`
- .NET 10 SDK

## Task

`Product` and `Category` are mapped only by conventions and attributes. You will
add the configuration classes, apply them, add a default string length that an
attribute can still override, store an enum as text and seed two rows.

## Instructions

Complete the following TODOs:

- TODO-00: `ProductConfiguration`: table "products", `Name` up to 120 characters.
- TODO-01: `Sku` up to 20 characters with a unique index.
- TODO-02: `Price` in column "unit_price", precision 18 and scale 2.
- TODO-03: Ignore `CachedLabel`.
- TODO-04: Apply all configurations from the assembly in `OnModelCreating`.
- TODO-05: Implement `DefaultStringLengthConvention` (200 characters).
- TODO-06: Register it in `ConfigureConventions` and store `ProductStatus` as text.
- TODO-07: Seed two categories with `HasData`.

Run the tests until they all pass.

## Running the Lab

```bash
dotnet test
```

## Bonus (Optional)

- TODO-08 (optional): `UseSeeding` in `ShopDbOptions` that adds a third category at runtime.
- TODO-09 (optional): A check constraint so the price cannot be negative.
- TODO-10 (optional): Set the default string length with `Properties<string>().HaveMaxLength`
  instead and see what happens to the `[MaxLength(50)]` on `Category.Slug`.
