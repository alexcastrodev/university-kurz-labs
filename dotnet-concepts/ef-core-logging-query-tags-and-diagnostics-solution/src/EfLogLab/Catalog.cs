using Microsoft.EntityFrameworkCore;

namespace EfLogLab;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public double Price { get; set; }
}

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}

public static class CatalogSeed
{
    public static void Apply(CatalogDbContext db)
    {
        db.Database.EnsureCreated();
        db.Products.AddRange(
            new Product { Name = "Keyboard", Price = 15 },
            new Product { Name = "Monitor", Price = 120 },
            new Product { Name = "Desk", Price = 300 });
        db.SaveChanges();
    }
}
