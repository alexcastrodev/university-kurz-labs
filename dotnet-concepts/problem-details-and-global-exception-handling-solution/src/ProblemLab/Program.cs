using ProblemLab;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(o => o.CustomizeProblemDetails = ctx =>
{
    ctx.ProblemDetails.Instance ??= ctx.HttpContext.Request.Path;
});
builder.Services.AddExceptionHandler<DomainExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/games/{id:int}", (int id) =>
    id == 1 ? Results.Ok(new { id, name = "Astro Vault" }) : throw new GameNotFoundException(id));

app.MapGet("/timeout", () => { throw new TimeoutException("db host db.internal:5432 timed out"); });

app.MapGet("/boom", () => { throw new InvalidOperationException("password=secret123"); });

app.Run();

public partial class Program;
