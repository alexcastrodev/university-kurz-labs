using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options, string tenantId) : DbContext(options)
{
    public const string UniqueOrderNumberIndex = "ix_orders_tenant_number";
    public const string TenantFilter = "Tenant";
    public const string SoftDeleteFilter = "SoftDelete";

    public string TenantId { get; } = tenantId;

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();
        order.Property(o => o.TenantId).HasMaxLength(50);
        order.Property(o => o.Number).HasMaxLength(20);
        order.Property(o => o.CustomerName).HasMaxLength(100);
        order.Property(o => o.Version).IsRowVersion();
        order.HasIndex(o => new { o.TenantId, o.Number }).IsUnique().HasDatabaseName(UniqueOrderNumberIndex);
        order.HasQueryFilter(TenantFilter, o => o.TenantId == TenantId)
            .HasQueryFilter(SoftDeleteFilter, o => !o.IsDeleted);

        var line = modelBuilder.Entity<OrderLine>();
        line.Property(l => l.Sku).HasMaxLength(20);
        line.Property(l => l.UnitPrice).HasPrecision(10, 2);
    }
}

// Used only by `dotnet ef`: it builds the model to compare with the migrations, it never connects.
public sealed class DesignTimeOrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<OrdersDbContext>().UseNpgsql("Host=localhost;Database=orders").Options, "design");
}
