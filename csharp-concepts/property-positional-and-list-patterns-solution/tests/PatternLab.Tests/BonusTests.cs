namespace PatternLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldSumASpanRecursivelyWithoutAllocatingSlices()
    {
        var numbers = Enumerable.Range(1, 2000).ToArray();

        var before = GC.GetAllocatedBytesForCurrentThread();
        var sum = Patterns.Sum(numbers);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(2001000, sum);
        Assert.True(allocated < 1024, $"Span slices should not allocate, but {allocated} bytes were allocated.");
    }

    [Fact]
    public void ShouldMatchAnyTypeThatDeclaresDeconstruct()
    {
        Assert.True(Patterns.IsGray(new Rgb(10, 10, 10)));
        Assert.False(Patterns.IsGray(new Rgb(10, 20, 10)));
    }
}
