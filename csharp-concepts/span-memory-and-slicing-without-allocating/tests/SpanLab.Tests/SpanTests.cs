namespace SpanLab.Tests;

public class SpanTests
{
    // Allocation checks use generous margins and a warm-up call: the tests tell "a handful of
    // bytes" apart from "a new array or string per call", not exact byte counts.
    private static long Allocated(Action action)
    {
        action();
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("   ", 0)]
    [InlineData("one", 1)]
    [InlineData("  the quick  brown fox ", 4)]
    public void ShouldCountWords(string text, int expected)
    {
        Assert.Equal(expected, Slicing.CountWords(text));
    }

    [Fact]
    public void ShouldCountWordsWithoutAllocating()
    {
        const string text = "the quick brown fox jumps over the lazy dog";

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Slicing.CountWords(text);
        });

        // Split(' ') would allocate a string[] and nine strings per call.
        Assert.True(allocated < 10_000, $"expected almost no allocation, got {allocated}");
    }

    [Fact]
    public void ShouldParseCommaSeparatedIntsIntoTheDestination()
    {
        Span<int> destination = stackalloc int[8];

        var count = Slicing.ParseInts("10,-20,30", destination);

        Assert.Equal(3, count);
        Assert.Equal([10, -20, 30], destination[..count].ToArray());
    }

    [Fact]
    public void ShouldParseASingleInt()
    {
        Span<int> destination = stackalloc int[2];

        Assert.Equal(1, Slicing.ParseInts("42", destination));
        Assert.Equal(42, destination[0]);
    }

    [Fact]
    public void ShouldParseWithoutAllocating()
    {
        var destination = new int[8];

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Slicing.ParseInts("1,2,3,4,5", destination);
        });

        Assert.True(allocated < 10_000, $"expected almost no allocation, got {allocated}");
    }

    [Fact]
    public void ShouldReturnAWritableViewOfTheMiddle()
    {
        var values = new[] { 1, 2, 3, 4, 5 };

        var middle = Slicing.Middle(values);
        middle[0] = 99;

        Assert.Equal(3, middle.Length);
        Assert.Equal([1, 99, 3, 4, 5], values);
    }

    [Fact]
    public void ShouldSumParamsSpanWithoutAllocating()
    {
        Assert.Equal(6, Slicing.Sum(1, 2, 3));
        Assert.Equal(0, Slicing.Sum());

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Slicing.Sum(i, 2, 3);
        });

        // params int[] would allocate 36 bytes per call (about 36_000 here).
        Assert.True(allocated < 10_000, $"expected almost no allocation, got {allocated}");
    }

    [Fact]
    public void ShouldAcceptAnArrayWhereAParamsSpanIsExpected()
    {
        Assert.Equal(10, Slicing.Sum(new[] { 1, 2, 3, 4 }));
    }

    [Fact]
    public void ShouldFormatBytesAsUpperHex()
    {
        Assert.Equal("00FF10AB", Slicing.ToHex([0x00, 0xFF, 0x10, 0xAB]));
        Assert.Equal("", Slicing.ToHex([]));
    }

    [Fact]
    public void ShouldAllocateOnlyTheResultString()
    {
        byte[] bytes = [1, 2, 3, 4, 5, 6, 7, 8];

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Slicing.ToHex(bytes);
        });

        // The result string is about 56 bytes. A char[] or a per-byte string would at least double it.
        Assert.True(allocated < 1_000 * 80, $"expected only the result string, got {allocated} bytes");
    }

    [Fact]
    public async Task ShouldReadMemoryAfterAnAwait()
    {
        var data = new[] { 1, 2, 3, 4, 5 };

        var sum = await Slicing.SumAsync(data.AsMemory(1..4));

        Assert.Equal(9, sum);
    }

    [Fact]
    public void ShouldFillAnInlineArrayThroughASpan()
    {
        var buffer = Buffers.Squares();
        Span<int> span = buffer;

        Assert.Equal([0, 1, 4, 9], span.ToArray());
    }
}
