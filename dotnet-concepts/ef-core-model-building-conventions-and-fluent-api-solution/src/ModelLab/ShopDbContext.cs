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

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShopDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new DefaultStringLengthConvention());
        configurationBuilder.Properties<ProductStatus>().HaveConversion<string>();
    }
}

public sealed class DefaultStringLengthConvention : IModelFinalizingConvention
{
    public const int DefaultLength = 200;

    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        foreach (var property in modelBuilder.Metadata.GetEntityTypes()
                     .SelectMany(entity => entity.GetDeclaredProperties())
                     .Where(property => property.ClrType == typeof(string)))
        {
            property.Builder.HasMaxLength(DefaultLength);
        }
    }
}

public sealed class PreConventionShopDbContext(DbContextOptions<PreConventionShopDbContext> options) : ShopDbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<string>().HaveMaxLength(DefaultStringLengthConvention.DefaultLength);
    }
}
