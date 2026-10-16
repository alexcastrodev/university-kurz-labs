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
        modelBuilder.Entity<Blog>().HasQueryFilter(TenantFilter, b => b.TenantId == _tenantId);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType)))
        {
            var e = Expression.Parameter(entityType.ClrType, "e");
            var isNotDeleted = Expression.Equal(
                Expression.Property(e, nameof(ISoftDelete.IsDeleted)), Expression.Constant(false));
            entityType.SetQueryFilter(SoftDeletionFilter, Expression.Lambda(isNotDeleted, e));
        }
    }
}
