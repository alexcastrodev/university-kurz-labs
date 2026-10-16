using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ModelLab;

public class ShopDbContext : DbContext
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
    {
    }

    protected ShopDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // TODO-04: Apply every IEntityTypeConfiguration<T> found in this assembly.
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // TODO-06: Register DefaultStringLengthConvention and store every ProductStatus as text.
    }
}

public sealed class DefaultStringLengthConvention : IModelFinalizingConvention
{
    public const int DefaultLength = 200;

    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        // TODO-05: Give every string property DefaultLength through the convention builder
        //          (property.Builder.HasMaxLength). Because it is a convention, attributes and
        //          Fluent calls must still be able to override it.
    }
}

public sealed class PreConventionShopDbContext(DbContextOptions<PreConventionShopDbContext> options) : ShopDbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        // TODO-10 (optional): Set the same default with configurationBuilder.Properties<string>()
        //          .HaveMaxLength(...) instead and read Category.Slug (which has [MaxLength(50)]).
    }
}
