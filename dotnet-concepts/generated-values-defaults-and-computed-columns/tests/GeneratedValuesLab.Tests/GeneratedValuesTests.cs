using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GeneratedValuesLab.Tests;

public sealed class GeneratedValuesTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<ShopDbContext> _options;

    public GeneratedValuesTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<ShopDbContext>().UseSqlite(_connection).Options;
        using var db = Db();
        ShopDatabase.Create(db);
    }

    private ShopDbContext Db() => new(_options);

    public void Dispose() => _connection.Dispose();

    private static Account NewAccount(string first = "Ana", string last = "Silva") => new() { FirstName = first, LastName = last };

    [Fact]
    public void ShouldReadBackTheStatusDefault()
    {
        using var db = Db();
        var account = NewAccount();
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.Equal("Active", account.Status);
    }

    [Fact]
    public void ShouldKeepAStatusThatWasSetExplicitly()
    {
        using var db = Db();
        var account = NewAccount();
        account.Status = "Closed";
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.Equal("Closed", account.Status);
    }

    [Fact]
    public void ShouldUseTheDefaultCreditsWhenNothingWasSet()
    {
        using var db = Db();
        var account = NewAccount();
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.Equal(10, account.Credits);
    }

    [Fact]
    public void ShouldNotSwallowAnIntentionalZeroCredits()
    {
        using var db = Db();
        var account = NewAccount();
        account.Credits = 0;
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.Equal(0, account.Credits);
    }

    [Fact]
    public void ShouldLetTheDatabaseClockFillCreatedAt()
    {
        using var db = Db();
        var account = NewAccount();
        db.Accounts.Add(account);

        db.SaveChanges();

        Assert.True(account.CreatedAt.Year >= 2024, $"CreatedAt was {account.CreatedAt:o}");
    }

    [Fact]
    public void ShouldComputeTheFullNameInTheDatabase()
    {
        using var db = Db();
        var account = NewAccount();
        db.Accounts.Add(account);
        db.SaveChanges();
        Assert.Equal("Ana Silva", account.FullName);

        account.LastName = "Souza";
        db.SaveChanges();

        Assert.Equal("Ana Souza", account.FullName);
    }

    [Fact]
    public void ShouldFilterOnTheComputedColumn()
    {
        using var db = Db();
        db.Accounts.AddRange(NewAccount("Ana", "Silva"), NewAccount("Bia", "Lima"));
        db.SaveChanges();

        var ids = db.Accounts.Where(a => a.FullName == "Bia Lima").Select(a => a.FirstName).ToList();

        Assert.Equal(["Bia"], ids);
    }

    [Fact]
    public void ShouldReadBackUpdatedAtSetByTheTrigger()
    {
        using var db = Db();
        var account = NewAccount();
        db.Accounts.Add(account);
        db.SaveChanges();
        Assert.Null(account.UpdatedAt);

        account.LastName = "Souza";
        db.SaveChanges();

        Assert.NotNull(account.UpdatedAt);
    }

    [Fact]
    public void ShouldApplyDefaultsToRowsInsertedOutsideEfCore()
    {
        using var db = Db();

        db.Database.ExecuteSqlRaw("INSERT INTO Accounts (FirstName, LastName) VALUES ('Raw', 'Writer')");

        var account = db.Accounts.AsNoTracking().Single();
        Assert.Equal("Active", account.Status);
        Assert.Equal(10, account.Credits);
        Assert.Equal("Raw Writer", account.FullName);
    }
}
