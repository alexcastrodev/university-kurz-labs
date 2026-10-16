using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace QueryFiltersLab;

public class AppDbContext(DbContextOptions<AppDbContext> options, string tenantId) : DbContext(options)
{
    // Share the names between the model and the queries: a typo in IgnoreQueryFilters is silent.
    public const string SoftDeletionFilter = "SoftDeletionFilter";
    public const string TenantFilter = "TenantFilter";

    // The filter lambda reads this field, EF Core turns it into a parameter per instance.
    private readonly string _tenantId = tenantId;

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO-00: Add a named filter (TenantFilter) on Blog: only rows whose TenantId equals
        // _tenantId. Use the HasQueryFilter overload that takes a name (EF Core 10).

        // TODO-01: Loop over the entity types that implement ISoftDelete and give each one a
        // named filter (SoftDeletionFilter) for "IsDeleted == false". The lambda has to be built
        // with System.Linq.Expressions and applied with entityType.SetQueryFilter(name, lambda).
    }
}
