namespace Orders.Api.Orders;

public sealed record Order(int Id, string Sku, int Quantity);

public sealed record PlaceOrderRequest(string? Sku, int Quantity);

public sealed class OrderRules(int maxQuantity)
{
    public int MaxQuantity { get; } = maxQuantity;

    public Dictionary<string, string[]> Validate(PlaceOrderRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            errors["sku"] = ["The SKU is required."];
        }

        if (request.Quantity < 1 || request.Quantity > MaxQuantity)
        {
            errors["quantity"] = [$"The quantity must be between 1 and {MaxQuantity}."];
        }

        return errors;
    }
}
