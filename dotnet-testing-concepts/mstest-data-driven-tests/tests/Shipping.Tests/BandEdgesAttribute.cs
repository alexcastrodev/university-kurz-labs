using System.Reflection;

namespace Shipping.Tests;

// TODO-05: A custom data source. For every band except the last, and for every zone, yield one row:
// (the band's UpToGrams, the zone). Read the edges from ShippingCalculator.Bands, so a new band gets its
// rows for free. GetDisplayName should name the row, for example "1000 g to 1001 g, Neighbor".
// Do not compute expected prices from Bands as well: a test that asks the code under test for its
// expectation agrees with any bug in it.
[AttributeUsage(AttributeTargets.Method)]
public sealed class BandEdgesAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo) =>
        throw new NotImplementedException("TODO-05: yield one row per band edge and zone.");

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) =>
        throw new NotImplementedException("TODO-05: name the row.");
}
