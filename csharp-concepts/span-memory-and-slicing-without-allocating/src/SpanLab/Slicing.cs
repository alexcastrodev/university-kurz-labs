namespace SpanLab;

public static class Slicing
{
    public static int CountWords(ReadOnlySpan<char> text)
    {
        // TODO-00: Count the words separated by one or more spaces without allocating. No Split
        //   and no Substring: use Trim, IndexOf and slicing on the span.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ParseInts(ReadOnlySpan<char> line, Span<int> destination)
    {
        // TODO-01: Parse a comma separated line such as "10,-20,30" into the destination and
        //   return how many ints were written. Slice each field and pass the span to int.Parse.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Span<int> Middle(int[] values)
    {
        // TODO-02: Return a writable view of the array without its first and last element.
        //   Use AsSpan with a range. Writing to the span must change the array.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Sum(params int[] values)
    {
        // TODO-03: Change the parameter to `params ReadOnlySpan<int>` (C# 13) so that calls such
        //   as Sum(1, 2, 3) do not allocate an array, then add up the values.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string ToHex(ReadOnlySpan<byte> bytes)
    {
        // TODO-04: Return the bytes as upper case hex ("00FF10AB"). Format into a stackalloc
        //   char buffer (or a heap buffer when the input is large) and allocate only the result
        //   string. byte.TryFormat(span, out _, "X2") writes without allocating.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static async Task<int> SumAsync(Memory<int> data)
    {
        // TODO-05: Yield with await Task.Yield(), then sum the data. A Span cannot live across
        //   an await, so read data.Span only after it, in a place that has no await after it.
        throw new NotImplementedException("Not implemented yet.");
    }
}
