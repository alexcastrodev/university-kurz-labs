namespace PipelineLab;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string Header = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext ctx)
    {
        // TODO-03: Use the incoming X-Correlation-Id header when present, otherwise generate
        // a new id (Guid in "N" format). Store it in ctx.Items["CorrelationId"] and write it to
        // the response header. Do this BEFORE calling next: once the response starts,
        // headers are read-only.
        await next(ctx);
    }
}
