using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JsonLab.Tests;

public sealed class JsonTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<ShopDbContext> _options;

    public JsonTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlite(_connection).Options;

        using var db = Db();
        db.Database.EnsureCreated();
        db.Customers.AddRange(
            new Customer
            {
                Id = 1, Name = "Ana", Tags = ["vip", "beta"],
                Address = new Address("Rua A", "Lisboa", "PT"),
                Shipping = [new Address("Dock 1", "Porto", "PT"), new Address("Calle 2", "Madrid", "ES")]
            },
            new Customer
            {
                Id = 2, Name = "Bruno", Tags = ["beta"],
                Address = new Address("Rua B", "Porto", "PT"),
                Billing = new Address("Rua C", "Braga", "PT")
            },
            new Customer
            {
                Id = 3, Name = "Clara", Address = new Address("Platz 3", "Berlin", "DE")
            });
        db.SaveChanges();
    }

    private ShopDbContext Db() => new(_options);

    public void Dispose() => _connection.Dispose();

    [Fact]
    public async Task ShouldStoreTheAddressAsOneJsonColumn()
    {
        await using var db = Db();

        var city = await db.Database
            .SqlQuery<string>($"SELECT json_extract(Address, '$.City') AS Value FROM Customers WHERE Id = 1")
            .SingleAsync();

        Assert.Equal("Lisboa", city);
    }

    [Fact]
    public async Task ShouldKeepTheOptionalBillingAddressNullable()
    {
        await using var db = Db();

        var customers = await db.Customers.OrderBy(c => c.Id).ToListAsync();

        Assert.Null(customers[0].Billing);
        Assert.Equal("Braga", customers[1].Billing!.City);
    }

    [Fact]
    public async Task ShouldRoundTripTheShippingCollection()
    {
        await using var db = Db();

        var ana = await db.Customers.SingleAsync(c => c.Id == 1);

        Assert.Equal(["Porto", "Madrid"], ana.Shipping.Select(s => s.City));
    }

    [Fact]
    public async Task ShouldFilterByAMemberInsideTheAddress()
    {
        await using var db = Db();

        Assert.Equal(["Bruno"], await CustomerQueries.NamesByCityAsync(db, "Porto"));
    }

    [Fact]
    public async Task ShouldFilterByAPrimitiveCollection()
    {
        await using var db = Db();

        Assert.Equal(["Ana", "Bruno"], await CustomerQueries.NamesWithTagAsync(db, "beta"));
        Assert.Equal(["Ana"], await CustomerQueries.NamesWithTagAsync(db, "vip"));
    }

    [Fact]
    public async Task ShouldFilterByAMemberOfTheComplexCollection()
    {
        await using var db = Db();

        Assert.Equal(["Ana"], await CustomerQueries.NamesShippingToAsync(db, "ES"));
        Assert.Empty(await CustomerQueries.NamesShippingToAsync(db, "FR"));
    }

    [Fact]
    public async Task ShouldReplaceTheComplexValueAndSaveIt()
    {
        await using (var db = Db())
        {
            await CustomerQueries.MoveAsync(db, 1, "Faro");
        }

        await using var check = Db();
        var ana = await check.Customers.SingleAsync(c => c.Id == 1);
        Assert.Equal(new Address("Rua A", "Faro", "PT"), ana.Address);
    }

    [Fact]
    public async Task ShouldUpdateOneJsonMemberInBulkWithoutLoadingEntities()
    {
        await using (var db = Db())
        {
            var rows = await CustomerQueries.AppendToCityAsync(db, "PT", "!");
            Assert.Equal(2, rows);
        }

        await using var check = Db();
        var cities = await check.Customers.OrderBy(c => c.Id).Select(c => c.Address.City).ToListAsync();
        Assert.Equal(["Lisboa!", "Porto!", "Berlin"], cities);
    }
}
