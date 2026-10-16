namespace PipelineLab;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string Header = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext ctx)
    {
        var id = ctx.Request.Headers[Header].FirstOrDefault() ?? Guid.NewGuid().ToString("N");
        ctx.Items["CorrelationId"] = id;
        // Set before next: once the response starts, headers are read-only.
        ctx.Response.Headers[Header] = id;
        await next(ctx);
    }
}
