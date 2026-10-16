using System.Linq.Expressions;

namespace ExprLab.Tests;

public class BonusTests
{
    private static readonly Order[] Data =
    [
        new(1, "Ana", 50m),
        new(2, "Bruno", 150m),
        new(3, "Ana", 250m),
        new(4, "Carla", 300m),
    ];

    [Fact]
    public void ShouldCombineTwoPredicatesWithAnd()
    {
        Expression<Func<Order, bool>> big = o => o.Total > 100;
        Expression<Func<Order, bool>> ana = x => x.Customer == "Ana";

        var combined = big.And(ana);

        Assert.Equal([3], Data.Where(combined.Compile()).Select(o => o.Id));
    }

    [Theory]
    [InlineData("Total", false, new[] { 1, 2, 3, 4 })]
    [InlineData("Total", true, new[] { 4, 3, 2, 1 })]
    [InlineData("Customer", false, new[] { 1, 3, 2, 4 })]
    public void ShouldOrderByAPropertyNameThroughTheProvider(string property, bool descending, int[] ids)
    {
        var log = new List<string>();
        var source = new LoggingQueryable<Order>(Data.AsQueryable(), log);

        var result = OrderQueries.OrderByProperty(source, property, descending).ToList();

        Assert.Contains(log, l => l.StartsWith("CreateQuery: OrderBy"));
        Assert.Equal(ids, result.Select(o => o.Id));
    }
}
