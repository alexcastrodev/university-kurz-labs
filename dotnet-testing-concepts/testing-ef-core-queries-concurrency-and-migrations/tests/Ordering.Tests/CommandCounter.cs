using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ordering.Tests;

// Records every query EF Core sends. Register a new instance per context with AddInterceptors.
// TODO-02: Override ReaderExecuting and ReaderExecutingAsync from DbCommandInterceptor. Add the command's
// CommandText to Commands and return the result unchanged, so the query still runs.
internal sealed class CommandCounter : DbCommandInterceptor
{
    public List<string> Commands { get; } = [];
}
