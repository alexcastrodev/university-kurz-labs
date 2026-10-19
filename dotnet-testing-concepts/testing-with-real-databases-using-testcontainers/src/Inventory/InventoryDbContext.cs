using Microsoft.EntityFrameworkCore;

namespace Inventory;

public sealed class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<Product>();
        product.Property(p => p.Sku).HasMaxLength(Product.SkuMaxLength);
        product.Property(p => p.Name).HasMaxLength(100);
        product.HasIndex(p => p.Sku).IsUnique();
    }
}
