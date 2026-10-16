using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab;

public class LegacyContext(DbContextOptions<LegacyContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // TODO-03: OrderLine has no Id. Declare its composite primary key (OrderId, LineNo).

        // TODO-04: Order.CustomerEmail must point at Customer.Email, not at Customer.Id.
        //   Configure Order -> Customer with HasForeignKey and HasPrincipalKey (an alternate key).
    }
}
