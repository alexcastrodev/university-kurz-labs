# Relationships, Keys, and Delete Behavior (Solution)

Official solution for the `relationships-keys-and-delete-behavior` lab.

## What changed from the starter

- `OnDelete(DeleteBehavior.SetNull)` on `Product.Supplier`: the default for an optional
  relationship is `ClientSetNull`, which only nulls tracked products and fails with
  `FOREIGN KEY constraint failed` when they were not loaded. `SetNull` writes
  `ON DELETE SET NULL` in the schema, so the database does it.
- `DeleteBehavior.Restrict` on `Product.Category` replaces the cascade a required
  relationship gets by default, so deleting a category with products throws
  `DbUpdateException`.
- `UsingEntity<ProductTag>` keeps the skip navigations `Tags` and `Products` and exposes a
  join class where `AddedOn` gets `DEFAULT (CURRENT_TIMESTAMP)`. Deleting a tag cascades to
  its join rows.
- `[PrimaryKey(nameof(OrderId), nameof(LineNo))]` (or `HasKey(x => new { x.OrderId, x.LineNo })`)
  declares the composite key; conventions never invent one.
- `HasForeignKey(o => o.CustomerEmail).HasPrincipalKey(c => c.Email)` creates the alternate
  key `AK_Customers_Email`. Alternate keys are read-only in EF Core, so changing the e-mail
  throws `InvalidOperationException`.
- `GetReviewProductIdAsync` selects `EF.Property<int>(r, "ProductId")`: `Review` has no such
  property, the column is a shadow foreign key.
- `DetachReviewAsync` removes the review from the loaded collection. The relationship is
  required, so the severed review is an orphan and EF Core issues the `DELETE`.

## Bonus solutions

- TODO-07 has no code: `db.Database.GenerateCreateScript()` shows `ON DELETE SET NULL` on
  `Products.SupplierId`, `ON DELETE RESTRICT` on `CategoryId`, and `ON DELETE CASCADE` on
  `Reviews` and `ProductTag`.

## Running

```bash
dotnet test
```
