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

        // TODO-03: Send EF Core logs to loggerFactory, and turn sensitive data logging and detailed
        // errors on or off according to the two flags.

        // TODO-04: Register the interceptors.

        return new CatalogDbContext(builder.Options);
    }
}
