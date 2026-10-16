using Microsoft.AspNetCore.Diagnostics;

namespace ProblemLab;

public class DomainExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        (int status, string title) = ex switch
        {
            GameNotFoundException => (404, "Game not found"),
            TimeoutException => (503, "Service temporarily unavailable"),
            _ => (0, ""),
        };
        if (status == 0) return false; // not ours, let the next handler (or the default) answer

        ctx.Response.StatusCode = status;
        if (ex is TimeoutException) ctx.Response.Headers.RetryAfter = "5";

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            Exception = ex,
            ProblemDetails =
            {
                Status = status,
                Title = title,
                // Only our own exception messages are safe to show.
                Detail = ex is GameNotFoundException ? ex.Message : null,
            },
        });
    }
}
