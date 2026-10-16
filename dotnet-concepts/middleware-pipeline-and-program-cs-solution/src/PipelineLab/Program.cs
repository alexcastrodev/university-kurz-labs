using PipelineLab;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Outermost: the only middleware that wraps everything after it.
app.Use(async (ctx, next) =>
{
    try { await next(ctx); }
    catch (Exception ex)
    {
        ctx.Response.StatusCode = 500;
        await ctx.Response.WriteAsync("caught: " + ex.Message);
    }
});

// 2. Correlation id before the gate, so even the 503 carries it.
app.UseMiddleware<CorrelationIdMiddleware>();

// 3. Maintenance gate: short-circuits by not calling next.
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

app.Map("/branch", branch => branch.Run(ctx =>
    ctx.Response.WriteAsync($"pathBase={ctx.Request.PathBase} path={ctx.Request.Path}")));

app.MapGet("/health", () => "ok");
app.MapGet("/hello", (HttpContext ctx) => $"hello {ctx.Items["CorrelationId"]}");
app.MapGet("/boom", () => { throw new InvalidOperationException("kaboom"); });

app.Run();

public partial class Program;
