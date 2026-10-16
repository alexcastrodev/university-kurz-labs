using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EfCoreAdoNetDapperAndNosql.Tests;

public sealed class DataAccessTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<AppDbContext> _options;

    public DataAccessTests()
    {
        _connection.Open();
        Schema.CreateAndSeed(_connection);
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;
    }

    private AppDbContext Db() => new(_options);

    public void Dispose() => _connection.Dispose();

    private long Count(string table)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = $"SELECT COUNT(*) FROM {table}";
        return (long)cmd.ExecuteScalar()!;
    }

    // TODO-00
    [Fact]
    public void AdoNet_ShouldFilterAndOrderByPrice()
    {
        var books = AdoNetProducts.FindByCategory(_connection, 1, 20);

        Assert.Equal(["Book B", "Book C"], books.Select(p => p.Name));
        Assert.Equal(25, books[0].UnitPrice);
    }

    [Fact]
    public void AdoNet_ShouldStoreHostileInputLiterally()
    {
        const string hostile = "x'); DROP TABLE Products; --";

        var id = AdoNetProducts.Add(_connection, 2, hostile, 5);

        Assert.True(id > 0);
        Assert.Equal(6, Count("Products"));
        Assert.Contains(AdoNetProducts.FindByCategory(_connection, 2, 0), p => p.Name == hostile && p.Id == id);
    }

    // TODO-01
    [Fact]
    public void AdoNet_ShouldCommitAllProducts()
    {
        AdoNetProducts.AddAll(_connection,
            [new Product { CategoryId = 3, Name = "N1", UnitPrice = 1 }, new Product { CategoryId = 3, Name = "N2", UnitPrice = 2 }]);

        Assert.Equal(7, Count("Products"));
    }

    [Fact]
    public void AdoNet_ShouldRollBackEverythingWhenOneProductFails()
    {
        var batch = new[]
        {
            new Product { CategoryId = 3, Name = "Fine", UnitPrice = 1 },
            new Product { CategoryId = 3, Name = "Negative", UnitPrice = -1 } // violates CHECK (unit_price >= 0)
        };

        Assert.Throws<SqliteException>(() => AdoNetProducts.AddAll(_connection, batch));

        Assert.Equal(5, Count("Products"));
    }

    // TODO-02
    [Fact]
    public void Dapper_ShouldMapUnderscoreColumnToProperty()
    {
        var games = DapperProducts.FindByCategory(_connection, 2, 0);

        Assert.Equal(["Game B", "Game A"], games.Select(p => p.Name));
        Assert.Equal([30.0, 60.0], games.Select(p => p.UnitPrice));
    }

    [Fact]
    public void Dapper_ShouldReturnTheSameRowsAsAdoNet()
    {
        var ado = AdoNetProducts.FindByCategory(_connection, 1, 10).Select(p => (p.Id, p.Name, p.UnitPrice));
        var dapper = DapperProducts.FindByCategory(_connection, 1, 10).Select(p => (p.Id, p.Name, p.UnitPrice));

        Assert.Equal(ado, dapper);
    }

    // TODO-03
    [Fact]
    public void Dapper_ShouldLoadCategoriesWithProductsInOneJoin()
    {
        var categories = DapperProducts.CategoriesWithProducts(_connection);

        Assert.Equal(["Books", "Games", "Empty"], categories.Select(c => c.Name));
        Assert.Equal(["Book A", "Book B", "Book C"], categories[0].Products.Select(p => p.Name));
        Assert.Equal(["Game B", "Game A"], categories[1].Products.Select(p => p.Name));
        Assert.Empty(categories[2].Products);
        Assert.Equal(10, categories[0].Products[0].UnitPrice);
    }

    // TODO-04
    [Fact]
    public void Ef_ShouldReturnTheSameRowsAsAdoNet()
    {
        using var db = Db();

        var ef = EfProducts.ListReadOnly(db, 1, 20);

        Assert.Equal(AdoNetProducts.FindByCategory(_connection, 1, 20).Select(p => p.Name), ef.Select(p => p.Name));
    }

    [Fact]
    public void Ef_ReadOnlyQueryShouldNotTrackEntities()
    {
        using var db = Db();

        EfProducts.ListReadOnly(db, 1, 0);

        Assert.Empty(db.ChangeTracker.Entries());
        Assert.Equal(3, db.Products.Where(p => p.CategoryId == 1).ToList().Count);
        Assert.Equal(3, db.ChangeTracker.Entries().Count()); // a normal query does track
    }

    // TODO-05
    [Fact]
    public void Mixed_ShouldCommitDapperUpdateInsideTheEfTransaction()
    {
        using var db = Db();

        var affected = EfProducts.RaisePrices(db, 1, 2, commit: true);

        Assert.Equal(3, affected);
        Assert.Equal([20.0, 50.0, 80.0], EfProducts.ListReadOnly(db, 1, 0).Select(p => p.UnitPrice));
    }

    [Fact]
    public void Mixed_ShouldRollBackTheDapperUpdateWithTheEfTransaction()
    {
        using var db = Db();

        var affected = EfProducts.RaisePrices(db, 1, 2, commit: false);

        Assert.Equal(3, affected); // Dapper did update the rows...
        Assert.Equal([10.0, 25.0, 40.0], EfProducts.ListReadOnly(db, 1, 0).Select(p => p.UnitPrice)); // ...but they were rolled back
    }

    // TODO-06 (bonus)
    private sealed record Shoe(string Name, string Color, int Size);

    [Fact]
    public void Documents_ShouldRoundTripAndReplace()
    {
        var store = new DocumentStore(_connection);

        store.Put("shoe:1", new Shoe("Runner", "red", 42));
        store.Put("shoe:1", new Shoe("Runner", "blue", 43));

        Assert.Equal(new Shoe("Runner", "blue", 43), store.Get<Shoe>("shoe:1"));
        Assert.Null(store.Get<Shoe>("shoe:404"));
        Assert.Equal(1, Count("Documents"));
    }

    [Fact]
    public void Documents_ShouldAllowDifferentShapesAndQueryByPath()
    {
        var store = new DocumentStore(_connection);
        store.Put("a", new { color = "red", size = 40 });
        store.Put("b", new { color = "blue" });
        store.Put("c", new { color = "red", tags = new[] { "sale" } });

        Assert.Equal(["a", "c"], store.FindKeys("$.color", "red"));
        Assert.Empty(store.FindKeys("$.color", "green"));
    }

    [Fact]
    public void Documents_ShouldNotEnforceReferences()
    {
        var store = new DocumentStore(_connection);

        store.Put("orphan", new { categoryId = 999 }); // a Products row with CategoryId 999 would be rejected

        Assert.NotNull(store.Get<Dictionary<string, int>>("orphan"));
        Assert.Throws<SqliteException>(() => AdoNetProductsWithForeignKeys());
    }

    private void AdoNetProductsWithForeignKeys()
    {
        using var pragma = _connection.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON";
        pragma.ExecuteNonQuery();
        AdoNetProducts.Add(_connection, 999, "Orphan", 1);
    }

    // TODO-07 (bonus)
    [Fact]
    public void Dapper_ShouldInsertManyRowsWithOneExecute()
    {
        var affected = DapperProducts.AddAll(_connection,
        [
            new Product { CategoryId = 3, Name = "B1", UnitPrice = 1 },
            new Product { CategoryId = 3, Name = "B2", UnitPrice = 2 },
            new Product { CategoryId = 3, Name = "B3", UnitPrice = 3 }
        ]);

        Assert.Equal(3, affected);
        Assert.Equal(8, Count("Products"));
    }
}
