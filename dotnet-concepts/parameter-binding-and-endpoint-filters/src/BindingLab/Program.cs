using BindingLab;

var builder = WebApplication.CreateBuilder(args);

// TODO-00: Every endpoint answers 500 right now, even the ones that look fine. Read the log
// for the first request, then register the missing service.

var app = builder.Build();

app.MapGet("/map", (Point p) => FormattableString.Invariant($"Point: {p.X}, {p.Y}"));

app.MapGet("/who", (Caller caller) => $"caller={caller.Name}");

app.MapGet("/search/{category}", ([AsParameters] SearchRequest r) => r);

// TODO-01: Make the source of `clock` explicit so a forgotten registration can never
// fall through to the request body.
app.MapGet("/time", (Clock clock) => clock.Now.Year);

// TODO-06: Attach OrderValidationFilter to this group.
app.MapGroup("/orders")
    .MapPost("/", (CreateOrder order) => Results.Created("/orders/1", order));

app.Run();

public partial class Program;
