using ClosuresLab;

namespace ClosuresLab.Tests;

public class FunctionsTests
{
    [Fact]
    public void ShouldCountUpOnEachCall()
    {
        var next = Functions.MakeCounter();

        Assert.Equal([1, 2, 3], new[] { next(), next(), next() });
    }

    [Fact]
    public void ShouldKeepCountersIndependent()
    {
        var first = Functions.MakeCounter();
        var second = Functions.MakeCounter();

        first();
        first();

        Assert.Equal(1, second());
    }

    [Fact]
    public void ShouldCaptureOneValuePerIteration()
    {
        var readers = Functions.MakeReaders(3);

        Assert.Equal([0, 1, 2], readers.Select(reader => reader()));
    }

    [Fact]
    public void ShouldSquareLazily() =>
        Assert.Equal([1, 4, 9], Functions.Squares([1, 2, 3]));

    [Fact]
    public void ShouldValidateTheArgumentWithoutEnumerating() =>
        Assert.Throws<ArgumentNullException>(() => Functions.Squares(null!));

    [Fact]
    public void ShouldParseWithTheMethodGroup()
    {
        var parse = Functions.IntParser();

        Assert.Equal(42, parse("42"));
        Assert.Throws<FormatException>(() => parse("x"));
    }

    [Fact]
    public void ShouldComputeOncePerArgument()
    {
        var calls = 0;
        var memoized = Functions.Memoize(n =>
        {
            calls++;
            return n * 10;
        });

        Assert.Equal(30, memoized(3));
        Assert.Equal(30, memoized(3));
        Assert.Equal(40, memoized(4));
        Assert.Equal(2, calls);
    }
}
