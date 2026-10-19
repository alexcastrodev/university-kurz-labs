namespace Ordering.Tests;

internal static class Seed
{
    public const string Acme = "acme";
    public const string Globex = "globex";

    public static async Task<Order> PlaceAsync(this TestDatabase database, string tenant, string number,
        string customer = "Customer", params NewLine[] lines)
    {
        await using var db = database.NewContext(tenant);
        return await new OrderService(db).PlaceAsync(number, customer, lines);
    }
}
