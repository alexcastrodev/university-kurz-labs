namespace LinqLab.Tests;

public class QueryTests
{
    private static readonly Customer Ana = new(1, "Ana", "ana@x.com");
    private static readonly Customer Bia = new(2, "Bia", "bia@x.com");
    private static readonly Customer Caio = new(3, "Caio", "caio@x.com");

    private static readonly Customer[] Customers = [Ana, Bia, Caio];

    private static readonly Order[] Orders =
    [
        new(10, 1, 100m),
        new(11, 1, 50m),
        new(12, 2, 30m),
        new(13, null, 999m),
    ];

    [Fact]
    public void ShouldKeepOnlyMatchingPairsInAnInnerJoin()
    {
        var rows = Queries.InnerJoin(Customers, Orders);

        Assert.Equal([("Ana", 100m), ("Ana", 50m), ("Bia", 30m)], rows);
    }

    [Fact]
    public void ShouldKeepCustomersWithoutOrdersInALeftJoin()
    {
        var rows = Queries.LeftJoin(Customers, Orders);

        Assert.Equal(
            [("Ana", (decimal?)100m), ("Ana", 50m), ("Bia", 30m), ("Caio", null)],
            rows);
    }

    [Fact]
    public void ShouldCountZeroOrdersWithAGroupJoin()
    {
        var rows = Queries.CountOrders(Customers, Orders);

        Assert.Equal([("Ana", 2), ("Bia", 1), ("Caio", 0)], rows);
    }

    [Fact]
    public void ShouldReturnAnEmptySequenceForAMissingKey()
    {
        var lookup = Queries.OrdersByCustomer(Orders);

        Assert.Equal(2, lookup[1].Count());
        Assert.Empty(lookup[42]);
    }

    [Fact]
    public void ShouldSnapshotTheSourceWhenBuildingTheLookup()
    {
        var source = new List<Order>(Orders);

        var lookup = Queries.OrdersByCustomer(source);
        source.Add(new Order(20, 1, 1m));

        Assert.Equal(2, lookup[1].Count());
    }

    [Fact]
    public void ShouldSubtractKnownEmailsIgnoringCaseWithoutDuplicates()
    {
        var known = new[] { Ana };
        var incoming = new[]
        {
            new Customer(9, "Ana again", "ANA@X.COM"),
            new Customer(10, "Dora", "dora@x.com"),
            new Customer(11, "Dora twin", "Dora@x.com"),
            new Customer(12, "Eli", "eli@x.com"),
        };

        var result = Queries.NewCustomers(known, incoming);

        Assert.Equal(["Dora", "Eli"], result.Select(c => c.Name));
    }

    [Fact]
    public void ShouldSumPerCustomerAndSkipOrdersWithoutOne()
    {
        var totals = Queries.TotalPerCustomer(Orders.Reverse());

        Assert.Equal([(1, 150m), (2, 30m)], totals);
    }
}
