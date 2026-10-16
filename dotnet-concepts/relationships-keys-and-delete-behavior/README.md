# Relationships, Keys, and Delete Behavior

## Goal

Configure EF Core relationships on SQLite and watch how the foreign key, the delete
behavior, composite and alternate keys, shadow keys and a many-to-many join entity change
what happens when rows disappear.

## Prerequisites

- C# and LINQ
- `async`/`await`
- Basic EF Core (`DbContext`, `DbSet`, `SaveChanges`)
- .NET 10 SDK

## Task

Two small models share the lab. `ShopContext` has categories, suppliers, products, reviews
and tags. `LegacyContext` has orders that point at a customer by e-mail and order lines
with a two-column key. The tests run against in-memory SQLite and use a **new context for
every check**, so the children are never loaded unless the test loads them. That is what
exposes the difference between `ClientSetNull` and `SetNull`.

## Instructions

Complete the following TODOs:

- TODO-00: Make deleting a supplier null `Product.SupplierId` in the database.
- TODO-01: Make deleting a category that still has products fail.
- TODO-02: Use `ProductTag` as the join entity with a `CURRENT_TIMESTAMP` default.
- TODO-03: Declare the composite key of `OrderLine`.
- TODO-04: Point `Order.CustomerEmail` at the alternate key `Customer.Email`.
- TODO-05: Read the shadow foreign key of a `Review`.
- TODO-06: Remove a review from `product.Reviews` and let EF Core delete the orphan.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-07 (optional): Print `Database.GenerateCreateScript()` for `ShopContext` and find the
  `ON DELETE` clause of each foreign key. Which one has none?
