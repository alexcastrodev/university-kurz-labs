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
        // TODO-00: Product.SupplierId is optional. Configure Product -> Supplier so that the
        //   DATABASE nulls the foreign key when a supplier is deleted, even if the products
        //   were never loaded (DeleteBehavior.SetNull).

        // TODO-01: Product.CategoryId is required, so the default is cascade. Configure
        //   Product -> Category so that deleting a category that still has products fails.

        // TODO-02: Product.Tags is a many-to-many. Keep the skip navigations but use ProductTag
        //   as the join entity (UsingEntity<ProductTag>) and give AddedOn the database default
        //   CURRENT_TIMESTAMP (HasDefaultValueSql).
    }
}
