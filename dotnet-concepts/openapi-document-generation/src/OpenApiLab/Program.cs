using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using OpenApiLab;

var builder = WebApplication.CreateBuilder(args);

// TODO-00: Register the OpenAPI services (document name "v1") and map the
// endpoint that serves /openapi/{documentName}.json. The options lambda is
// where TODO-02 and TODO-06 go.

// TODO-02: Add a document transformer: title "Game Store API", version "2.0.0"
// and one server with url "https://api.example.com".

// TODO-05: Register a second document named "admin".

var app = builder.Build();

var games = app.MapGroup("/games");

// TODO-01: Describe this endpoint: summary "Gets a game by id", operation name
// (operationId) "GetGame", tag "Games", and typed responses: 200 with GameDto
// and 404.
games.MapGet("/{id:int}", (int id) =>
    id == 1 ? Results.Ok(new GameDto(1, "Astro Vault", 19.99m)) : Results.NotFound());

games.MapGet("/", () => new[] { new GameDto(1, "Astro Vault", 19.99m) });

// TODO-03: Mark only this endpoint as deprecated with an operation transformer
// attached to the endpoint.
games.MapGet("/legacy", () => "old");

// TODO-04: Keep this endpoint out of the OpenAPI document (it must still work).
app.MapGet("/internal/health", () => "ok");

// TODO-05: This endpoint must appear only in the "admin" document.
app.MapGet("/admin/stats", () => new { games = 1 });

app.Run();

public partial class Program { }
