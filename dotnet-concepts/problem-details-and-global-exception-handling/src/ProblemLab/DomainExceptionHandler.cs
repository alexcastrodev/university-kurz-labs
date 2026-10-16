using Microsoft.AspNetCore.Diagnostics;

namespace ProblemLab;

public class DomainExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        (int status, string title) = ex switch
        {
            // TODO-04: Map GameNotFoundException to (404, "Game not found").
            // TODO-05: Map TimeoutException to (503, "Service temporarily unavailable").
            _ => (0, ""),
        };
        if (status == 0) return false; // not ours, let the next handler (or the default) answer

        ctx.Response.StatusCode = status;
        // TODO-06: A timeout is temporary, so tell clients to retry in 5 seconds (Retry-After header).

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
