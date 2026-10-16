using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ModelLab.Tests;

public sealed class ModelTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public ModelTests() => _connection.Open();

    public void Dispose() => _connection.Dispose();

    private static IModel Model()
    {
        var options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlite("DataSource=:memory:").Options;
        using var db = new ShopDbContext(options);
        return db.GetService<IDesignTimeModel>().Model;
    }

    private static IEntityType ProductType() => Model().FindEntityType(typeof(Product))!;
    private static IEntityType CategoryType() => Model().FindEntityType(typeof(Category))!;

    private ShopDbContext CreateDb()
    {
        var db = new ShopDbContext(ShopDbOptions.Create(_connection));
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public void ShouldMapProductToTheProductsTableAndKeepTheConventionNameForCategory()
    {
        Assert.Equal("products", ProductType().GetTableName());
        Assert.Equal("Categories", CategoryType().GetTableName());
    }

    [Fact]
    public void ShouldLetTheFluentApiBeatTheMaxLengthAttribute()
    {
        Assert.Equal(120, ProductType().FindProperty(nameof(Product.Name))!.GetMaxLength());
    }

    [Fact]
    public void ShouldLimitAndIndexTheSkuUniquely()
    {
        var sku = ProductType().FindProperty(nameof(Product.Sku))!;

        Assert.Equal(20, sku.GetMaxLength());
        Assert.Contains(ProductType().GetIndexes(), i => i.IsUnique && i.Properties.Single() == sku);
    }

    [Fact]
    public void ShouldRenameAndSizeThePriceColumn()
    {
        var price = ProductType().FindProperty(nameof(Product.Price))!;

        Assert.Equal("unit_price", price.GetColumnName(StoreObjectIdentifier.Table("products")));
        Assert.Equal(18, price.GetPrecision());
        Assert.Equal(2, price.GetScale());
    }

    [Fact]
    public void ShouldNotMapCachedLabel()
    {
        Assert.Null(ProductType().FindProperty(nameof(Product.CachedLabel)));
    }

    [Fact]
    public void ShouldGiveEveryOtherStringTheDefaultLength()
    {
        Assert.Equal(200, CategoryType().FindProperty(nameof(Category.Name))!.GetMaxLength());
        Assert.Equal(200, ProductType().FindProperty(nameof(Product.Note))!.GetMaxLength());
    }

    [Fact]
    public void ShouldLetAnAttributeOverrideTheDefaultLength()
    {
        Assert.Equal(50, CategoryType().FindProperty(nameof(Category.Slug))!.GetMaxLength());
    }

    [Fact]
    public void ShouldStoreTheStatusEnumAsText()
    {
        var status = ProductType().FindProperty(nameof(Product.Status))!;

        Assert.Equal(typeof(string), status.GetProviderClrType());
    }

    [Fact]
    public void ShouldSeedTwoCategoriesWithHasData()
    {
        using var db = CreateDb();

        var names = db.Categories.Where(c => c.Id <= 2).OrderBy(c => c.Id).Select(c => c.Name).ToList();

        Assert.Equal(["Books", "Games"], names);
    }

    [Fact]
    public void ShouldRejectADuplicateSku()
    {
        using var db = CreateDb();
        db.Products.Add(new Product { Name = "A", Sku = "X-1", CategoryId = 1 });
        db.Products.Add(new Product { Name = "B", Sku = "X-1", CategoryId = 1 });

        Assert.Throws<DbUpdateException>(() => db.SaveChanges());
    }
}
