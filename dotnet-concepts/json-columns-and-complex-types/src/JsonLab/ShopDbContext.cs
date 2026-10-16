using Microsoft.EntityFrameworkCore;

namespace JsonLab;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Customer>(customer =>
        {
            // TODO-00: Map the required Address as a complex property stored in one JSON column.
            // TODO-01: Map the optional Billing address the same way.
            // TODO-02: Map Shipping (List<Address>) as a complex collection in a JSON column.
            //          Tags needs no configuration: find out why.
        });
}
