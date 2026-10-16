using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JsonLab.Tests;

public sealed class BonusTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<ShopDbContext> _options;

    public BonusTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlite(_connection).Options;
        using var db = new ShopDbContext(_options);
        db.Database.EnsureCreated();
        db.Customers.AddRange(
            new Customer { Id = 1, Name = "Ana", Address = new Address("Rua A", "Lisboa", "PT"), Shipping = [new Address("Dock 1", "Porto", "PT")] },
            new Customer { Id = 2, Name = "Bruno", Address = new Address("Rua B", "Porto", "PT") });
        db.SaveChanges();
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task ShouldNotInventAShippingElementForACustomerWithAnEmptyList()
    {
        await using var db = new ShopDbContext(_options);

        var rows = await CustomerQueries.SetFirstShippingCityAsync(db, "X");

        Assert.Equal(1, rows);
        await using var check = new ShopDbContext(_options);
        Assert.Empty((await check.Customers.SingleAsync(c => c.Id == 2)).Shipping);
        Assert.Equal("X", (await check.Customers.SingleAsync(c => c.Id == 1)).Shipping[0].City);
    }

    [Fact]
    public async Task ShouldReadTheStoredJsonDocument()
    {
        await using var db = new ShopDbContext(_options);

        var json = await CustomerQueries.StoredAddressJsonAsync(db, 1);

        Assert.Contains("\"City\":\"Lisboa\"", json);
    }
}
