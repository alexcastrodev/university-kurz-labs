using Microsoft.AspNetCore.Mvc;
using BindingLab;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Clock>();

var app = builder.Build();

app.MapGet("/map", (Point p) => FormattableString.Invariant($"Point: {p.X}, {p.Y}"));

app.MapGet("/who", (Caller caller) => $"caller={caller.Name}");

app.MapGet("/search/{category}", ([AsParameters] SearchRequest r) => r);

app.MapGet("/time", ([FromServices] Clock clock) => clock.Now.Year);

app.MapGroup("/orders")
    .AddEndpointFilter<OrderValidationFilter>()
    .MapPost("/", (CreateOrder order) => Results.Created("/orders/1", order));

app.Run();

public partial class Program;
