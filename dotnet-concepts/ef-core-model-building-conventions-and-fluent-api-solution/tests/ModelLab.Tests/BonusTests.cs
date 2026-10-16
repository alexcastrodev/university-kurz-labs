using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ModelLab.Tests;

public sealed class BonusTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public BonusTests() => _connection.Open();

    public void Dispose() => _connection.Dispose();

    [Fact]
    public void ShouldSeedAtRuntimeWithUseSeedingNextToHasData()
    {
        using var db = new ShopDbContext(ShopDbOptions.Create(_connection));
        db.Database.EnsureCreated();

        Assert.Equal(["Books", "Games", "Added at runtime"], db.Categories.OrderBy(c => c.Id).Select(c => c.Name).ToList());
    }

    [Fact]
    public void ShouldEnforceANonNegativePriceInTheDatabase()
    {
        using var db = new ShopDbContext(ShopDbOptions.Create(_connection));
        db.Database.EnsureCreated();
        db.Products.Add(new Product { Name = "Bad", Sku = "B-1", Price = -1, CategoryId = 1 });

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
    }

    [Fact]
    public void ShouldLoseTheAttributeWhenTheDefaultIsPreConventionConfiguration()
    {
        var options = new DbContextOptionsBuilder<PreConventionShopDbContext>().UseSqlite("DataSource=:memory:").Options;
        using var db = new PreConventionShopDbContext(options);

        var slug = db.Model.FindEntityType(typeof(Category))!.FindProperty(nameof(Category.Slug))!;

        Assert.Equal(200, slug.GetMaxLength());
    }
}
