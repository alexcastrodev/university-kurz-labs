using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab;

public class LegacyContext(DbContextOptions<LegacyContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Order>().HasOne(o => o.Customer).WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerEmail).HasPrincipalKey(c => c.Email);
    }
}
