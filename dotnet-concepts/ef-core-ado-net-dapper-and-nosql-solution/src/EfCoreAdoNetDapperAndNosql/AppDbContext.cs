using Microsoft.EntityFrameworkCore;

namespace EfCoreAdoNetDapperAndNosql;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().Property(p => p.UnitPrice).HasColumnName("unit_price");
        modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne().HasForeignKey(p => p.CategoryId);
    }
}
