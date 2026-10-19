namespace PartialLab.Tests;

public class PartialTests
{
    [Fact]
    public void ShouldMergeSealedAndInterfacesAcrossParts()
    {
        Assert.True(typeof(Order).IsSealed);
        Assert.Equal(typeof(Entity), typeof(Order).BaseType);
        Assert.Contains(typeof(IComparable<Order>), typeof(Order).GetInterfaces());
    }

    [Fact]
    public void ShouldSortOrdersByTotal()
    {
        var orders = new List<Order>
        {
            new() { Id = 1, Total = 30m },
            new() { Id = 2, Total = 10m },
            new() { Id = 3, Total = 20m },
        };

        orders.Sort();

        Assert.Equal([2, 3, 1], orders.Select(o => o.Id));
        Assert.Equal(1, orders[0].CompareTo(null));
    }

    [Fact]
    public void ShouldSkipArgumentEvaluationWhenHookIsNotImplemented()
    {
        var quiet = new Quiet();

        var result = quiet.Save();

        Assert.Equal("saved", result);
        Assert.Equal(0, quiet.ExpensiveCalls);
    }

    [Fact]
    public void ShouldRunImplementedHook()
    {
        var loud = new Loud();

        var result = loud.Save("order 7");

        Assert.Equal("saved", result);
        Assert.Equal(["saving:order 7"], loud.Log);
    }

    [Fact]
    public void ShouldTrimTitleInPartialProperty()
    {
        var widget = new Widget(2) { Title = "  hello  " };

        Assert.Equal("hello", widget.Title);
    }

    [Fact]
    public void ShouldMultiplyIndexByWidgetSize()
    {
        var widget = new Widget(3);

        Assert.Equal(0, widget[0]);
        Assert.Equal(12, widget[4]);
    }

    [Fact]
    public void ShouldDescribeWithTheExtendedPartialMethod()
    {
        var widget = new Widget(5) { Title = " box " };

        Assert.Equal("box:5", widget.Describe());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-4)]
    public void ShouldRejectNonPositiveSizeInPartialConstructor(int size)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Widget(size));
    }

    [Fact]
    public void ShouldRaiseChangedUntilHandlerIsRemoved()
    {
        var widget = new Widget(1);
        var seen = new List<string>();
        Action<string> handler = seen.Add;

        widget.Changed += handler;
        widget.Rename("  first ");
        widget.Changed -= handler;
        widget.Rename("second");

        Assert.Equal(["first"], seen);
    }

    [Fact]
    public void ShouldRenameWithoutHandlers()
    {
        var widget = new Widget(1);

        widget.Rename("solo");

        Assert.Equal("solo", widget.Title);
    }

    [Fact]
    public void ShouldCallGeneratedHalfThroughPrefixFromYourPart()
    {
        var greeter = new Greeter("Hi, ");

        Assert.Equal("Hi, Ana!", greeter.Greet("Ana"));
    }

    [Fact]
    public void ShouldGreetEveryNameSeparatedBySpace()
    {
        var greeter = new Greeter("Hi ");

        Assert.Equal("Hi Ana! Hi Bob!", greeter.GreetAll(["Ana", "Bob"]));
        Assert.Equal("", greeter.GreetAll([]));
    }
}
