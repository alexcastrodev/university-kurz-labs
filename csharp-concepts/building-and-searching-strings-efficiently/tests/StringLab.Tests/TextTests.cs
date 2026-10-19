using System.Globalization;
using System.Text;

namespace StringLab.Tests;

public class TextTests
{
    // Allocation checks use generous margins and a warm-up call: the tests tell "a handful of
    // bytes" apart from "a new string per piece", not exact byte counts.
    private static long Allocated(Action action)
    {
        action();
        var before = GC.GetAllocatedBytesForCurrentThread();
        action();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static int ChunkCount(StringBuilder builder)
    {
        var count = 0;
        foreach (var _ in builder.GetChunks()) count++;
        return count;
    }

    // ---- TODO-00 ----

    [Fact]
    public void ShouldBuildCsv()
    {
        var rows = new List<string[]> { new[] { "id", "name" }, new[] { "1", "Ana" } };

        Assert.Equal("id,name\n1,Ana\n", Text.BuildCsv(rows));
    }

    [Fact]
    public void ShouldBuildEmptyCsvFromNoRows()
    {
        Assert.Equal("", Text.BuildCsv([]));
    }

    [Fact]
    public void ShouldBuildLargeCsvWithoutQuadraticCopying()
    {
        var rows = Enumerable.Range(0, 2_000).Select(i => new[] { i.ToString(), "name", "x" }).ToList();
        string csv = "";

        var allocated = Allocated(() => csv = Text.BuildCsv(rows));

        Assert.EndsWith("1999,name,x\n", csv);
        // `+=` over ~6,000 pieces would copy well over 100 MB; a builder stays near the output size.
        Assert.True(allocated < 2_000_000, $"expected a few hundred KB at most, got {allocated}");
    }

    // ---- TODO-01 ----

    [Fact]
    public void ShouldPreSizeTheBuilderToOneChunk()
    {
        var builder = Text.CreateBuilder(1_000);
        builder.Append('x', 1_000);

        Assert.True(builder.Capacity >= 1_000);
        Assert.Equal(1, ChunkCount(builder));
    }

    [Fact]
    public void ShouldWriteEveryChunkInOrder()
    {
        var builder = new StringBuilder();   // default capacity: grows by adding chunks
        for (var i = 0; i < 200; i++) builder.Append(i).Append(';');
        Assert.True(ChunkCount(builder) > 1);

        var writer = new StringWriter();
        Text.WriteChunks(builder, writer);

        Assert.Equal(builder.ToString(), writer.ToString());
    }

    // ---- TODO-02 ----

    [Theory]
    [InlineData("ORD", 42, "ORD-00042")]
    [InlineData("A", 0, "A-00000")]
    [InlineData("", 99999, "-99999")]
    public void ShouldFormatTag(string prefix, int number, string expected)
    {
        Assert.Equal(expected, Text.FormatTag(prefix, number));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100_000)]
    public void ShouldRejectNumbersOutOfRange(int number)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Text.FormatTag("ORD", number));
    }

    [Fact]
    public void ShouldAllocateOnlyTheResultString()
    {
        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Text.FormatTag("ORD", i);
        });

        // One 9-char string is ~40 bytes; a builder or ToString().PadLeft() chain costs several times that.
        Assert.True(allocated < 80_000, $"expected about one string per call, got {allocated}");
    }

    // ---- TODO-03 ----

    [Theory]
    [InlineData("hello world", -1)]
    [InlineData("a<b", 1)]
    [InlineData("tom & jerry", 4)]
    [InlineData("say \"hi\"", 4)]
    [InlineData("it's", 2)]
    [InlineData("", -1)]
    public void ShouldFindFirstForbiddenCharacter(string text, int expected)
    {
        Assert.Equal(expected, Text.IndexOfForbidden(text));
    }

    [Fact]
    public void ShouldFindForbiddenCharacterInLongText()
    {
        var text = new string('a', 5_000) + ">" + new string('b', 100);

        Assert.Equal(5_000, Text.IndexOfForbidden(text));
    }

    [Fact]
    public void ShouldNotAllocateWhenSearchingForForbiddenCharacters()
    {
        const string text = "a perfectly clean line of text";

        var allocated = Allocated(() =>
        {
            for (var i = 0; i < 1_000; i++) Text.IndexOfForbidden(text);
        });

        // Creating the set inside the method would allocate on every call.
        Assert.True(allocated < 10_000, $"expected almost no allocation, got {allocated}");
    }

    // ---- TODO-04 ----

    [Theory]
    [InlineData("all ok, then FATAL happened", 13)]
    [InlineData("an Error occurred", 3)]
    [InlineData("kernel panic", 7)]
    [InlineData("nothing to see", -1)]
    [InlineData("", -1)]
    public void ShouldFindFirstKeywordIgnoringCase(string text, int expected)
    {
        Assert.Equal(expected, Text.IndexOfKeyword(text));
    }

    [Fact]
    public void ShouldMatchOrdinallyRegardlessOfCurrentCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            // In Turkish, culture-aware comparison pairs 'I' with a dotless i, so "PANIC" would not
            // match "panic". SearchValues is ordinal and ignores the current culture.
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

            Assert.Equal(0, Text.IndexOfKeyword("PANIC"));
            Assert.Equal(4, Text.IndexOfKeyword("log Panic now"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }
}
