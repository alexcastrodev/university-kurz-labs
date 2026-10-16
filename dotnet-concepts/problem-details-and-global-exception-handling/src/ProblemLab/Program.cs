using ProblemLab;

var builder = WebApplication.CreateBuilder(args);

// TODO-00: Register the problem details service. Customize it so that every
// document gets `Instance` set to the request path (when not already set).

// TODO-01: Register DomainExceptionHandler as an exception handler.

var app = builder.Build();

// TODO-02: Add the exception handler middleware so thrown exceptions become responses.

// TODO-03: Add status code pages so empty 4xx/5xx responses (like an unknown route) get a body.

app.MapGet("/games/{id:int}", (int id) =>
    id == 1 ? Results.Ok(new { id, name = "Astro Vault" }) : throw new GameNotFoundException(id));

app.MapGet("/timeout", () => { throw new TimeoutException("db host db.internal:5432 timed out"); });

app.MapGet("/boom", () => { throw new InvalidOperationException("password=secret123"); });

app.Run();

public partial class Program;
