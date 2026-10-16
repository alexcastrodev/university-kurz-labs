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
        // TODO-05: Count the command and remember its text, then return the result unchanged.
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        // TODO-06: The async path is a separate hook. Count it too.
        return ValueTask.FromResult(result);
    }
}
