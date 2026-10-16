using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GeneratedValuesLab.Tests;

public sealed class BonusTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<ShopDbContext> _options;

    public BonusTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlite(_connection).Options;
        using var db = new ShopDbContext(_options);
        ShopDatabase.Create(db);
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public void ShouldGenerateTheTicketRefOnTheClient()
    {
        using var db = new ShopDbContext(_options);
        var first = new Ticket();
        var second = new Ticket();
        db.Tickets.AddRange(first, second);

        db.SaveChanges();

        Assert.NotEqual(Guid.Empty, first.Ref);
        Assert.NotEqual(first.Ref, second.Ref);
    }

    [Fact]
    public void ShouldComputeInitialsWithoutStoringThem()
    {
        using var db = new ShopDbContext(_options);
        var account = new Account { FirstName = "ana", LastName = "silva" };
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.Equal("AS", account.Initials);
    }
}
