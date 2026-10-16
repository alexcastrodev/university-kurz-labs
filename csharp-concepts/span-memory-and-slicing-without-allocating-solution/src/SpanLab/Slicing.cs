using System.Runtime.InteropServices;

namespace SpanLab;

public static class Slicing
{
    public static int CountWords(ReadOnlySpan<char> text)
    {
        var remaining = text.Trim();
        var count = 0;

        while (!remaining.IsEmpty)
        {
            count++;
            var space = remaining.IndexOf(' ');
            if (space < 0) break;
            remaining = remaining[(space + 1)..].TrimStart();
        }

        return count;
    }

    public static int ParseInts(ReadOnlySpan<char> line, Span<int> destination)
    {
        var count = 0;

        while (!line.IsEmpty)
        {
            var comma = line.IndexOf(',');
            var field = comma < 0 ? line : line[..comma];
            destination[count++] = int.Parse(field);
            line = comma < 0 ? default : line[(comma + 1)..];
        }

        return count;
    }

    public static Span<int> Middle(int[] values) => values.AsSpan(1..^1);

    public static int Sum(params ReadOnlySpan<int> values)
    {
        var total = 0;
        foreach (var value in values) total += value;
        return total;
    }

    public static string ToHex(ReadOnlySpan<byte> bytes)
    {
        Span<char> buffer = bytes.Length <= 128 ? stackalloc char[bytes.Length * 2] : new char[bytes.Length * 2];

        for (var i = 0; i < bytes.Length; i++) bytes[i].TryFormat(buffer[(i * 2)..], out _, "X2");

        return new string(buffer);
    }

    public static async Task<int> SumAsync(Memory<int> data)
    {
        await Task.Yield();
        return Sum(data.Span);
    }

    public static bool StartsWithZero(this ReadOnlySpan<int> span) => !span.IsEmpty && span[0] == 0;

    public static ReadOnlySpan<byte> AsBytes(ReadOnlySpan<int> values) => MemoryMarshal.AsBytes(values);
}
