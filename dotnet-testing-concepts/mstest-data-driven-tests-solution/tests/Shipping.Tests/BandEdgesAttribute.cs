using System.Reflection;

namespace Shipping.Tests;

// Generates one row per band edge and zone: the last gram of a band. The inputs come from
// ShippingCalculator.Bands, so a new band gets its rows for free. The expected prices must not come from
// there too: a test that computes its expectation with the code under test agrees with any bug in it.
// The test below asserts a property instead (one more gram never costs less).
[AttributeUsage(AttributeTargets.Method)]
public sealed class BandEdgesAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo) =>
        from band in ShippingCalculator.Bands
        where band.UpToGrams < ShippingCalculator.MaxWeightGrams
        from zone in Enum.GetValues<Zone>()
        select new object?[] { band.UpToGrams, zone };

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) =>
        data is null ? null : $"{data[0]} g to {(int)data[0]! + 1} g, {data[1]}";
}
