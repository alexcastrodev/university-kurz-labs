using Microsoft.EntityFrameworkCore;

namespace JsonLab;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Customer>(customer =>
        {
            customer.ComplexProperty(c => c.Address, address => address.ToJson());
            customer.ComplexProperty(c => c.Billing, address => address.ToJson());
            customer.ComplexCollection(c => c.Shipping, address => address.ToJson());
        });
}
