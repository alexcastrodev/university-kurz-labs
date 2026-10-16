namespace BindingLab;

public class OrderValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var order = ctx.GetArgument<CreateOrder>(0);

        var errors = new Dictionary<string, string[]>();
        if (string.IsNullOrWhiteSpace(order.Name)) errors["name"] = ["Name is required."];
        if (order.Quantity <= 0) errors["quantity"] = ["Quantity must be positive."];
        if (errors.Count > 0) return Results.ValidationProblem(errors);

        ctx.Arguments[0] = order with { Name = order.Name.Trim() };
        return await next(ctx);
    }
}
