using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab;

public class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>().HasOne(p => p.Supplier).WithMany(s => s.Products)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<Product>().HasMany(p => p.Tags).WithMany(t => t.Products)
            .UsingEntity<ProductTag>(j => j.Property(e => e.AddedOn).HasDefaultValueSql("CURRENT_TIMESTAMP"));
    }
}
