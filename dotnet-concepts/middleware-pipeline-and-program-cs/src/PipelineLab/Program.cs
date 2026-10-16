using PipelineLab;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// TODO-00: Add an inline middleware (app.Use) that wraps the rest of the pipeline in
// try/catch and answers 500 with the body "caught: <exception message>".
// Decide where it must go so it can see exceptions from everything after it.

// Maintenance gate: short-circuits by not calling next.
app.Use(async (ctx, next) =>
{
    if (app.Configuration.GetValue<bool>("Maintenance:Enabled") && ctx.Request.Path != "/health")
    {
        ctx.Response.StatusCode = 503;
        await ctx.Response.WriteAsync("maintenance");
        return;
    }
    await next(ctx);
});

// TODO-01: The 503 above has no X-Correlation-Id header. The two registrations are in the
// wrong order: fix it so the correlation id is also present on short-circuited responses.
app.UseMiddleware<CorrelationIdMiddleware>();

// TODO-02: Map "/branch" to a separate branch whose terminal delegate (Run) writes
// "pathBase=<PathBase> path=<Path>" as the response body.

app.MapGet("/health", () => "ok");
app.MapGet("/hello", (HttpContext ctx) => $"hello {ctx.Items["CorrelationId"]}");
app.MapGet("/boom", () => { throw new InvalidOperationException("kaboom"); });

app.Run();

public partial class Program;
