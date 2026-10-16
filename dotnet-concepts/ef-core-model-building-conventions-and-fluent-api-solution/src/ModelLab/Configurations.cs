using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ModelLab;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", table => table.HasCheckConstraint("CK_products_price", "unit_price >= 0"));
        builder.Property(p => p.Name).HasMaxLength(120);
        builder.Property(p => p.Sku).HasMaxLength(20);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.Property(p => p.Price).HasColumnName("unit_price").HasPrecision(18, 2);
        builder.Ignore(p => p.CachedLabel);
    }
}

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder) =>
        builder.HasData(
            new Category { Id = 1, Name = "Books", Slug = "books" },
            new Category { Id = 2, Name = "Games", Slug = "games" });
}
