namespace BindingLab;

public class OrderValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        // TODO-05: Return Results.ValidationProblem(...) when Name is blank (key "name") or
        // Quantity is not positive (key "quantity"). Otherwise replace argument 0 with a copy
        // whose Name is trimmed, so the handler only ever sees clean data.
        return await next(ctx);
    }
}
