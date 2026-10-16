using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab.Tests;

public sealed class ShopTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly DbContextOptions<ShopContext> _options;

    public ShopTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<ShopContext>().UseSqlite(_connection).Options;
        using var db = NewContext();
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private ShopContext NewContext() => new(_options);

    private (int CategoryId, int SupplierId, int ProductId) Seed()
    {
        using var db = NewContext();
        var category = new Category { Name = "Boards" };
        var supplier = new Supplier { Name = "Acme" };
        var product = new Product { Name = "Deck", Category = category, Supplier = supplier };
        db.Add(product);
        db.SaveChanges();
        return (category.Id, supplier.Id, product.Id);
    }

    [Fact]
    public async Task ShouldKeepProductsAndNullTheForeignKeyWhenSupplierIsDeletedWithoutLoadingThem()
    {
        var ids = Seed();

        await using (var db = NewContext())
            await new ShopService(db).DeleteSupplierAsync(ids.SupplierId);

        await using var check = NewContext();
        var product = await check.Products.SingleAsync();
        Assert.Null(product.SupplierId);
    }

    [Fact]
    public async Task ShouldRefuseToDeleteACategoryThatStillHasProducts()
    {
        var ids = Seed();

        await using var db = NewContext();
        db.Remove(await db.Categories.SingleAsync(c => c.Id == ids.CategoryId));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task ShouldFillAddedOnFromTheDatabaseDefaultWhenAProductIsTagged()
    {
        var ids = Seed();
        await using (var db = NewContext())
        {
            var product = await db.Products.Include(p => p.Tags).SingleAsync(p => p.Id == ids.ProductId);
            product.Tags.Add(new Tag { Name = "new" });
            await db.SaveChangesAsync();
        }

        await using var check = NewContext();
        var link = await check.Set<ProductTag>().SingleAsync();
        Assert.True(link.AddedOn.Year >= 2024);
    }

    [Fact]
    public async Task ShouldReadTheShadowForeignKeyOfAReview()
    {
        var ids = Seed();
        int reviewId;
        await using (var db = NewContext())
        {
            var review = new Review { Text = "great", Product = await db.Products.SingleAsync() };
            db.Add(review);
            await db.SaveChangesAsync();
            reviewId = review.Id;
        }

        await using var check = NewContext();
        Assert.Equal(ids.ProductId, await new ShopService(check).GetReviewProductIdAsync(reviewId));
    }

    [Fact]
    public async Task ShouldDeleteAReviewWhenItIsRemovedFromTheProductCollection()
    {
        var ids = Seed();
        int reviewId;
        await using (var db = NewContext())
        {
            var review = new Review { Text = "meh", Product = await db.Products.SingleAsync() };
            db.Add(review);
            await db.SaveChangesAsync();
            reviewId = review.Id;
        }

        await using (var db = NewContext())
            await new ShopService(db).DetachReviewAsync(ids.ProductId, reviewId);

        await using var check = NewContext();
        Assert.Empty(check.Reviews);
        Assert.Single(check.Products);
    }

    [Fact]
    public async Task ShouldDeleteTheJoinRowsWhenATagIsDeleted()
    {
        var ids = Seed();
        await using (var db = NewContext())
        {
            var product = await db.Products.Include(p => p.Tags).SingleAsync(p => p.Id == ids.ProductId);
            product.Tags.Add(new Tag { Name = "sale" });
            await db.SaveChangesAsync();
        }

        await using (var db = NewContext())
        {
            db.Remove(await db.Tags.SingleAsync());
            await db.SaveChangesAsync();
        }

        await using var check = NewContext();
        Assert.Empty(check.Set<ProductTag>());
        Assert.Single(check.Products);
    }
}
