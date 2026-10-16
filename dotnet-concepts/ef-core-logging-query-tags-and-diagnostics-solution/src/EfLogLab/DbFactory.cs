using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EfLogLab;

public static class DbFactory
{
    public static CatalogDbContext Create(
        SqliteConnection connection,
        ILoggerFactory loggerFactory,
        bool sensitiveData = false,
        bool detailedErrors = false,
        params IInterceptor[] interceptors)
    {
        var builder = new DbContextOptionsBuilder<CatalogDbContext>().UseSqlite(connection);

        builder.UseLoggerFactory(loggerFactory)
            .EnableSensitiveDataLogging(sensitiveData)
            .EnableDetailedErrors(detailedErrors)
            .AddInterceptors(interceptors);

        return new CatalogDbContext(builder.Options);
    }

    public static CatalogDbContext CreateWithLogTo(SqliteConnection connection, Action<string> sink)
    {
        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite(connection)
            .LogTo(sink, [RelationalEventId.CommandExecuted], LogLevel.Information, DbContextLoggerOptions.None)
            .Options;
        return new CatalogDbContext(options);
    }
}
