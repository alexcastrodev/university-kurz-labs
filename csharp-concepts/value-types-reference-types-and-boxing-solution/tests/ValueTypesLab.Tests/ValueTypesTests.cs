using System.Runtime.CompilerServices;

namespace ValueTypesLab.Tests;

public class ValueTypesTests
{
    // Allocation checks use generous margins: the runtime may allocate a little for its own
    // reasons, so we compare orders of magnitude, never exact byte counts.
    private static long Allocated(Action action)
    {
        action();
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    [Fact]
    public void ShouldNotChangeTheCallersCounterWhenPassedByValue()
    {
        var counter = new Counter();

        var result = Semantics.IncrementCopy(counter);

        Assert.Equal(1, result);
        Assert.Equal(0, counter.Value);
    }

    [Fact]
    public void ShouldChangeTheCallersCounterWhenPassedByRef()
    {
        var counter = new Counter();

        Semantics.IncrementRef(ref counter);
        Semantics.IncrementRef(ref counter);

        Assert.Equal(2, counter.Value);
    }

    [Fact]
    public void ShouldDeclareTemperatureAsReadonlyStruct()
    {
        Assert.True(typeof(Temperature).IsDefined(typeof(IsReadOnlyAttribute), false));
    }

    [Fact]
    public void ShouldReturnANewTemperatureAndKeepTheOriginal()
    {
        var cold = new Temperature { Celsius = 5 };

        var warm = cold.WithCelsius(25);

        Assert.Equal(25, warm.Celsius);
        Assert.Equal(5, cold.Celsius);
    }

    [Fact]
    public void ShouldKeepIncrementsMadeThroughAField()
    {
        Assert.Equal(2, new Holder().IncrementTwice());
    }

    [Fact]
    public void ShouldBoxEveryValueIntoItsOwnObject()
    {
        const int count = 1_000;
        var values = Enumerable.Range(0, count).ToArray();

        var allocated = Allocated(() => Boxing.BoxAll(values));

        Assert.True(allocated >= count * 24, $"expected at least {count * 24} bytes, got {allocated}");
    }

    [Fact]
    public void ShouldUnboxBackToTheSameValues()
    {
        var values = new[] { 4, 8, 15, 16, 23, 42 };

        Assert.Equal(values, Boxing.UnboxAll(Boxing.BoxAll(values)));
    }

    [Fact]
    public void ShouldSumOnTheStackWithoutHeapAllocation()
    {
        Assert.Equal(5_050, Boxing.SumOnStack(100));

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Boxing.SumOnStack(100);
        });

        // A heap int[100] per call would be about 1_000 * 424 bytes.
        Assert.True(allocated < 10_000, $"expected almost no allocation, got {allocated}");
    }

    [Fact]
    public void ShouldBoxANullableOnlyWhenItHasAValue()
    {
        Assert.Null(Boxing.BoxNullable(null));
        Assert.Equal(7, Assert.IsType<int>(Boxing.BoxNullable(7)));
    }

    // Bonus tests (TODO-07 to TODO-09 exist only in the solution).

    [Fact]
    public void ShouldComputeLengthPassingTheVectorByReadonlyReference()
    {
        Assert.Equal(3, Semantics.Length(new Vector3D(1, 2, 2)));
    }

    [Fact]
    public void ShouldAllocateMoreWhenFormatBoxesTheArgument()
    {
        var boxed = Boxing.AllocatedByFormat(10_000);
        var interpolated = Boxing.AllocatedByInterpolation(10_000);

        Assert.True(boxed > interpolated, $"format {boxed} should exceed interpolation {interpolated}");
    }

    [Fact]
    public void ShouldIncrementAListElementInPlace()
    {
        var counters = new List<Counter> { new() };

        Semantics.IncrementFirst(counters);

        Assert.Equal(1, counters[0].Value);
    }
}
