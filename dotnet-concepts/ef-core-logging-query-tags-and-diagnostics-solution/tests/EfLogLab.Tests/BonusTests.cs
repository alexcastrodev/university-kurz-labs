using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EfLogLab.Tests;

public sealed class BonusTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public BonusTests()
    {
        _connection.Open();
        using var seed = DbFactory.Create(_connection, LoggerFactory.Create(_ => { }));
        CatalogSeed.Apply(seed);
    }

    public void Dispose() => _connection.Dispose();

    [Fact]
    public void ShouldAddTheSourceFileAndLineWithCallSiteTags()
    {
        using var db = DbFactory.Create(_connection, LoggerFactory.Create(_ => { }));

        var sql = CatalogQueries.ExpensiveWithCallSite(db, 20).ToQueryString();

        Assert.Contains("-- Catalog: expensive products", sql);
        Assert.Matches(@"-- File: .+:\d+", sql);
    }

    [Fact]
    public void ShouldLogOnlyExecutedCommandsThroughLogTo()
    {
        var lines = new List<string>();
        using var db = DbFactory.CreateWithLogTo(_connection, lines.Add);

        CatalogQueries.Expensive(db, 20).ToList();

        var line = Assert.Single(lines);
        Assert.Contains("Executed DbCommand", line);
        Assert.Contains("FROM \"Products\"", line);
    }
}
