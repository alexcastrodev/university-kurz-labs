using System.Buffers;
using System.Text;

namespace StringLab;

public static class Text
{
    public static string BuildCsv(IReadOnlyList<string[]> rows)
    {
        // TODO-00: Build the CSV with a StringBuilder, not with `+=`. Fields of a row are joined
        //   with ',' and every row (including the last) ends with '\n'. No rows gives "".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static StringBuilder CreateBuilder(int expectedLength)
    {
        // TODO-01a: Return a builder that can hold expectedLength chars in a single chunk, so
        //   appending that much text never allocates a second chunk.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void WriteChunks(StringBuilder builder, TextWriter writer)
    {
        // TODO-01b: Write the builder content to the writer WITHOUT calling builder.ToString().
        //   Use GetChunks() and write each chunk's Span.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string FormatTag(string prefix, int number)
    {
        // TODO-02: Return "<prefix>-<number as 5 digits, zero padded>", e.g. ("ORD", 42) gives
        //   "ORD-00042". Allocate only the final string with string.Create, passing the data as
        //   the state argument so the lambda stays static. Throw ArgumentOutOfRangeException
        //   when number is below 0 or above 99999.
        throw new NotImplementedException("Not implemented yet.");
    }

    // TODO-03: Add a `private static readonly SearchValues<char>` field with the forbidden
    //   characters < > & " ' and use it in IndexOfForbidden. Create it once, not per call.
    public static int IndexOfForbidden(ReadOnlySpan<char> text)
    {
        // TODO-03: Return the index of the first forbidden character, or -1.
        throw new NotImplementedException("Not implemented yet.");
    }

    // TODO-04: Add a `private static readonly SearchValues<string>` field with "error", "fatal"
    //   and "panic", compared with StringComparison.OrdinalIgnoreCase.
    public static int IndexOfKeyword(ReadOnlySpan<char> text)
    {
        // TODO-04: Return the index where the first keyword starts, or -1. Matching is ordinal,
        //   so the current culture must not change the result.
        throw new NotImplementedException("Not implemented yet.");
    }
}
