using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenApiLab;

var builder = WebApplication.CreateBuilder(args);

// TODO-00: document "v1" plus the transformers.
builder.Services.AddOpenApi(options =>
{
    // TODO-02: document transformer for title, version and server.
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Info.Title = "Game Store API";
        document.Info.Version = "2.0.0";
        document.Servers = [new OpenApiServer { Url = "https://api.example.com" }];
        return Task.CompletedTask;
    });

    // TODO-06 (optional): schema transformer for every decimal.
    options.AddSchemaTransformer((schema, context, ct) =>
    {
        if (context.JsonTypeInfo.Type == typeof(decimal))
            schema.Description = "Money in euros";
        return Task.CompletedTask;
    });
});

// TODO-05: second document named "admin".
builder.Services.AddOpenApi("admin");

var app = builder.Build();

// TODO-00: serve /openapi/{documentName}.json
app.MapOpenApi();

var games = app.MapGroup("/games");

// TODO-01: summary, name, tag and typed responses.
games.MapGet("/{id:int}", (int id) =>
        id == 1 ? Results.Ok(new GameDto(1, "Astro Vault", 19.99m)) : Results.NotFound())
    .WithSummary("Gets a game by id")
    .WithName("GetGame")
    .WithTags("Games")
    .Produces<GameDto>()
    .Produces(StatusCodes.Status404NotFound);

games.MapGet("/", () => new[] { new GameDto(1, "Astro Vault", 19.99m) });

// TODO-03: mark only this endpoint as deprecated.
games.MapGet("/legacy", () => "old")
    .AddOpenApiOperationTransformer((operation, context, ct) =>
    {
        operation.Deprecated = true;
        return Task.CompletedTask;
    });

// TODO-04: keep it out of the document.
app.MapGet("/internal/health", () => "ok").ExcludeFromDescription();

// TODO-05: only in the "admin" document.
app.MapGet("/admin/stats", () => new { games = 1 }).WithGroupName("admin");

app.Run();

public partial class Program { }
