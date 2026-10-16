using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RelationshipsLab.Tests;

public sealed class LegacyTests : IDisposable
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly DbContextOptions<LegacyContext> _options;

    public LegacyTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<LegacyContext>().UseSqlite(_connection).Options;
        using var db = NewContext();
        db.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private LegacyContext NewContext() => new(_options);

    [Fact]
    public async Task ShouldStoreTwoLinesWithTheSameLineNumberInDifferentOrders()
    {
        await using var db = NewContext();
        db.AddRange(
            new OrderLine { OrderId = 1, LineNo = 1, Sku = "A" },
            new OrderLine { OrderId = 2, LineNo = 1, Sku = "B" });
        await db.SaveChangesAsync();

        Assert.Equal(2, await db.OrderLines.CountAsync());
    }

    [Fact]
    public async Task ShouldRejectTheSameOrderAndLineNumberTwice()
    {
        await using var db = NewContext();

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            db.AddRange(
                new OrderLine { OrderId = 1, LineNo = 1, Sku = "A" },
                new OrderLine { OrderId = 1, LineNo = 1, Sku = "B" });
            await db.SaveChangesAsync();
        });
    }

    [Fact]
    public async Task ShouldRejectAnOrderWhoseEmailMatchesNoCustomer()
    {
        await using var db = NewContext();
        db.Add(new Order { CustomerEmail = "nobody@example.com" });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task ShouldJoinOrdersToCustomersThroughTheEmail()
    {
        await using (var db = NewContext())
        {
            var ana = new Customer { Email = "ana@example.com" };
            ana.Orders.Add(new Order());
            db.Add(ana);
            await db.SaveChangesAsync();
        }

        await using var check = NewContext();
        var order = await check.Orders.Include(o => o.Customer).SingleAsync();
        Assert.Equal("ana@example.com", order.CustomerEmail);
    }

    [Fact]
    public async Task ShouldNotAllowTheAlternateKeyToChange()
    {
        await using var db = NewContext();
        var ana = new Customer { Email = "ana@example.com" };
        db.Add(ana);
        await db.SaveChangesAsync();

        ana.Email = "ana@new.example.com";

        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }
}
