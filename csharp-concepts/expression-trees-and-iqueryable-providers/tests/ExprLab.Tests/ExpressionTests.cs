using System.Linq.Expressions;

namespace ExprLab.Tests;

public class ExpressionTests
{
    private static readonly Order[] Data =
    [
        new(1, "Ana", 50m),
        new(2, "Bruno", 150m),
        new(3, "Ana", 250m),
        new(4, "Carla", 300m),
    ];

    [Fact]
    public void ShouldBuildTheSameTreeTheCompilerWouldBuild()
    {
        Expression<Func<Order, bool>> expected = o => o.Total > 100;

        var built = Builders.TotalGreaterThan(100);

        Assert.Equal(expected.ToString(), built.ToString());
        Assert.Equal(ExpressionType.GreaterThan, built.Body.NodeType);
        Assert.True(built.Compile()(new Order(1, "Ana", 101m)));
        Assert.False(built.Compile()(new Order(1, "Ana", 100m)));
    }

    [Fact]
    public void ShouldKeepTheValueAsAConstantInTheConstantVersion()
    {
        Assert.Contains("150", Builders.TotalAboveConstant(150).ToString());
    }

    [Fact]
    public void ShouldReferenceTheVariableInsteadOfItsValueInTheCapturedVersion()
    {
        var tree = Builders.TotalAboveCaptured(150);

        Assert.DoesNotContain("150", tree.ToString());
        Assert.Contains("min", tree.ToString());
        Assert.Equal([3, 4], Data.Where(tree.Compile()).Select(o => o.Id));
    }

    [Fact]
    public void ShouldReplaceOnlyTheMatchingParameter()
    {
        var from = Expression.Parameter(typeof(int), "x");
        var to = Expression.Parameter(typeof(int), "y");
        var other = Expression.Parameter(typeof(int), "z");

        var visitor = new ReplaceParameter(from, to);

        Assert.Same(to, visitor.Visit(from));
        Assert.Same(other, visitor.Visit(other));
    }

    [Fact]
    public void ShouldCombineTwoPredicatesWithOr()
    {
        Expression<Func<Order, bool>> big = o => o.Total > 200;
        Expression<Func<Order, bool>> ana = x => x.Customer == "Ana";

        var combined = big.Or(ana);

        Assert.Single(combined.Parameters);
        Assert.Equal([1, 3, 4], Data.Where(combined.Compile()).Select(o => o.Id));
    }

    [Fact]
    public void ShouldCollectMemberNamesInOrderWithoutDuplicates()
    {
        Expression<Func<Order, bool>> e = o => o.Total > 100 && o.Customer == "Ana" && o.Total < 300;

        Assert.Equal(["Total", "Customer"], MemberCollector.Collect(e));
    }

    [Fact]
    public void ShouldLogEachOperatorAndTheFinalExecution()
    {
        var log = new List<string>();
        var query = new LoggingQueryable<Order>(Data.AsQueryable(), log);

        var big = query.Where(o => o.Total > 100).OrderBy(o => o.Customer);

        Assert.Equal(["CreateQuery: Where", "CreateQuery: OrderBy"], log);

        Assert.Equal(3, big.Count());
        Assert.StartsWith("Execute:", log[2]);
        Assert.Contains("Where", log[2]);
        Assert.Contains("OrderBy", log[2]);
    }

    [Fact]
    public void ShouldLogEnumerationAndReturnTheResults()
    {
        var log = new List<string>();
        var query = new LoggingQueryable<Order>(Data.AsQueryable(), log);

        var result = query.Where(o => o.Total > 100).ToList();

        Assert.Equal([2, 3, 4], result.Select(o => o.Id));
        Assert.StartsWith("enumerate:", log[^1]);
    }

    [Theory]
    [InlineData(null, null, 0, new[] { 1, 2, 3, 4 })]
    [InlineData("Ana", null, 1, new[] { 1, 3 })]
    [InlineData(null, 200, 1, new[] { 3, 4 })]
    [InlineData("Ana", 200, 2, new[] { 3 })]
    public void ShouldAddOneWherePerFilterThatHasAValue(string? customer, int? min, int wheres, int[] ids)
    {
        var log = new List<string>();
        var source = new LoggingQueryable<Order>(Data.AsQueryable(), log);

        var result = OrderQueries.Filter(source, customer, min).ToList();

        Assert.Equal(wheres, log.Count(l => l == "CreateQuery: Where"));
        Assert.Equal(ids, result.Select(o => o.Id));
    }
}
