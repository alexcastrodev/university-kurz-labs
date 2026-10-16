using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EfLogLab;

public sealed class CommandCounter : DbCommandInterceptor
{
    public int Count { get; private set; }

    public List<string> Sql { get; } = [];

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        Record(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Record(command);
        return ValueTask.FromResult(result);
    }

    private void Record(DbCommand command)
    {
        Count++;
        Sql.Add(command.CommandText);
    }
}
