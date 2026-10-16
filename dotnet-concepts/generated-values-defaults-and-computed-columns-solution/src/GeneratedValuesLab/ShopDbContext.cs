using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace GeneratedValuesLab;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(account =>
        {
            account.Property(a => a.Status).HasDefaultValue("Active");
            account.Property(a => a.Credits).HasDefaultValue(10).HasSentinel(-1);
            account.Property(a => a.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            account.Property(a => a.FullName).HasComputedColumnSql("FirstName || ' ' || LastName", stored: true);
            account.Property(a => a.UpdatedAt).ValueGeneratedOnAddOrUpdate();
            account.ToTable(table => table.UseSqlReturningClause(false));
            account.Property(a => a.Initials)
                .HasComputedColumnSql("upper(substr(FirstName, 1, 1)) || upper(substr(LastName, 1, 1))", stored: false);
        });

        modelBuilder.Entity<Ticket>().Property(t => t.Ref).HasValueGenerator<GuidValueGenerator>();
    }
}
