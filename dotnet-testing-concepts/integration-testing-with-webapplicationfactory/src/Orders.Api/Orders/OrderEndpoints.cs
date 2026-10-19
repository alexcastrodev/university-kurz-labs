namespace Orders.Api.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/orders");

        orders.MapPost("/", async (PlaceOrderRequest request, OrderRules rules, IInventoryClient inventory,
            OrderStore store, CancellationToken ct) =>
        {
            var errors = rules.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var available = await inventory.GetAvailableAsync(request.Sku!, ct);
            if (available < request.Quantity)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Not enough stock",
                    detail: $"Only {available} of {request.Sku} available.");
            }

            var order = store.Add(request.Sku!, request.Quantity);
            return Results.Created($"/orders/{order.Id}", order);
        });

        orders.MapGet("/", (OrderStore store) => store.All());

        orders.MapGet("/{id:int}", (int id, OrderStore store) =>
            store.Find(id) is { } order
                ? Results.Ok(order)
                : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Order not found"));

        // The newest order moves every time someone orders, so this is a temporary redirect (302).
        orders.MapGet("/latest", (OrderStore store) =>
            store.Latest() is { } order
                ? Results.Redirect($"/orders/{order.Id}")
                : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "No orders yet"));

        return app;
    }
}
