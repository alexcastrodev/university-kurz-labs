using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EfLogLab.Tests;

public sealed class DiagnosticsTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly ListLoggerProvider _logs = new();
    private readonly ILoggerFactory _loggerFactory;

    public DiagnosticsTests()
    {
        _connection.Open();
        _loggerFactory = LoggerFactory.Create(b => b.AddProvider(_logs).SetMinimumLevel(LogLevel.Trace));
        using var seed = DbFactory.Create(_connection, LoggerFactory.Create(_ => { }));
        CatalogSeed.Apply(seed);
    }

    public void Dispose()
    {
        _loggerFactory.Dispose();
        _connection.Dispose();
    }

    private CatalogDbContext NewDb(bool sensitive = false, bool detailed = false, params IInterceptor[] interceptors) =>
        DbFactory.Create(_connection, _loggerFactory, sensitive, detailed, interceptors);

    private string? ExecutedCommandMessage() =>
        _logs.Entries.LastOrDefault(e => e.EventId.Id == RelationalEventId.CommandExecuted.Id)?.Message;

    [Fact]
    public void ShouldRecordCategoryLevelAndFormattedMessage()
    {
        var logger = _loggerFactory.CreateLogger("Demo.Category");

        logger.LogWarning("hello {Name}", "world");

        var entry = Assert.Single(_logs.Entries, e => e.Category == "Demo.Category");
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal("hello world", entry.Message);
    }

    [Fact]
    public void ShouldPrintParametersAndSqlWithoutExecuting()
    {
        using var db = NewDb();

        var sql = CatalogQueries.ExpensiveSql(db, 20);

        Assert.Contains(".param set @min 20", sql);
        Assert.Contains("WHERE \"p\".\"Price\" > @min", sql);
        Assert.Contains("ORDER BY \"p\".\"Name\"", sql);
        Assert.Null(ExecutedCommandMessage());
    }

    [Fact]
    public void ShouldPutAccumulatedTagsAsCommentsAboveTheSql()
    {
        using var db = NewDb();

        var sql = CatalogQueries.Expensive(db, 20).ToQueryString();

        Assert.StartsWith("-- Catalog: expensive products", sql.Split(Environment.NewLine).First(l => l.StartsWith("--")));
        Assert.Contains("-- Called from the price list screen", sql);
        Assert.True(sql.IndexOf("-- Catalog", StringComparison.Ordinal) < sql.IndexOf("SELECT", StringComparison.Ordinal));
    }

    [Fact]
    public void ShouldLogTheTagAsPartOfTheExecutedCommand()
    {
        using var db = NewDb();

        var rows = CatalogQueries.Expensive(db, 20).ToList();

        Assert.Equal(2, rows.Count);
        Assert.Contains("-- Catalog: expensive products", ExecutedCommandMessage());
    }

    [Fact]
    public void ShouldHideParameterValuesByDefault()
    {
        using var db = NewDb();

        CatalogQueries.Expensive(db, 20).ToList();

        var message = ExecutedCommandMessage();
        Assert.Contains("@min='?'", message);
        Assert.DoesNotContain("@min='20'", message);
    }

    [Fact]
    public void ShouldShowParameterValuesWhenSensitiveDataLoggingIsOn()
    {
        using var db = NewDb(sensitive: true);

        CatalogQueries.Expensive(db, 20).ToList();

        Assert.Contains("@min='20'", ExecutedCommandMessage());
    }

    [Theory]
    [InlineData(false, "The data is NULL")]
    [InlineData(true, "property 'Product.Name'")]
    public void ShouldNameTheNullPropertyOnlyWithDetailedErrors(bool detailed, string expected)
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        using (var setup = connection.CreateCommand())
        {
            setup.CommandText = "CREATE TABLE Products (Id INTEGER PRIMARY KEY, Name TEXT NULL, Price REAL NOT NULL);" +
                                "INSERT INTO Products (Name, Price) VALUES (NULL, 1);";
            setup.ExecuteNonQuery();
        }
        using var db = DbFactory.Create(connection, _loggerFactory, detailedErrors: detailed);

        var ex = Assert.Throws<InvalidOperationException>(() => db.Products.ToList());

        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public void ShouldCountSyncCommands()
    {
        var counter = new CommandCounter();
        using var db = NewDb(interceptors: counter);

        db.Products.ToList();

        Assert.Equal(1, counter.Count);
        Assert.Contains("FROM \"Products\"", counter.Sql[0]);
    }

    [Fact]
    public async Task ShouldCountAsyncCommandsToo()
    {
        var counter = new CommandCounter();
        await using var db = NewDb(interceptors: counter);

        await db.Products.ToListAsync();

        Assert.Equal(1, counter.Count);
    }

    [Fact]
    public void ShouldExposeTheNPlusOnePattern()
    {
        var counter = new CommandCounter();
        using var db = NewDb(interceptors: counter);

        var rows = CatalogQueries.LoadOneByOne(db);

        Assert.Equal(3, rows);
        Assert.Equal(1 + rows, counter.Count);
    }
}
