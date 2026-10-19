namespace Shipping.Tests;

// Test data shared by more than one test class lives in its own class.
public static class ShippingCases
{
    // TestDataRow<T> carries a display name (and categories, or an ignore message) for each row.
    public static IEnumerable<TestDataRow<(decimal OrderTotal, Zone Zone, decimal Expected)>> Thresholds =>
    [
        new((99.99m, Zone.Domestic, 5.00m)) { DisplayName = "one cent below the threshold pays" },
        new((100.00m, Zone.Domestic, 0m)) { DisplayName = "exactly the threshold ships free" },
        new((100.00m, Zone.Neighbor, 7.50m)) { DisplayName = "the threshold does not apply to neighbors" },
        new((250.00m, Zone.International, 12.50m)) { DisplayName = "nor to international orders" },
    ];
}
