using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModelLab;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // TODO-00: Map Product to the table "products" and allow 120 characters in Name.
        //          Name also carries [MaxLength(50)]: which source wins?

        // TODO-01: Sku is limited to 20 characters and has a unique index.

        // TODO-02: Store Price in a column called "unit_price" with precision 18 and scale 2.

        // TODO-03: CachedLabel is a runtime-only value, keep it out of the model.

        // TODO-09 (optional): Add a check constraint "CK_products_price" so unit_price >= 0
        //          (hint: the ToTable overload that takes a table builder).
    }
}

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // TODO-07: Seed two categories with HasData: Id 1 "Books" (slug "books")
        //          and Id 2 "Games" (slug "games"). HasData needs explicit keys.
    }
}
