using System.Buffers;
using System.Text;

namespace StringLab;

public static class Text
{
    private static readonly SearchValues<char> Forbidden = SearchValues.Create("<>&\"'");

    private static readonly SearchValues<string> Keywords =
        SearchValues.Create(["error", "fatal", "panic"], StringComparison.OrdinalIgnoreCase);

    public static string BuildCsv(IReadOnlyList<string[]> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
        {
            for (var i = 0; i < row.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(row[i]);
            }
            sb.Append('\n');
        }
        return sb.ToString();
    }

    public static StringBuilder CreateBuilder(int expectedLength) => new(expectedLength);

    public static void WriteChunks(StringBuilder builder, TextWriter writer)
    {
        foreach (var chunk in builder.GetChunks())
            writer.Write(chunk.Span);
    }

    public static string FormatTag(string prefix, int number)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(number);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(number, 99999);

        return string.Create(prefix.Length + 6, (prefix, number), static (span, state) =>
        {
            state.prefix.CopyTo(span);
            span[state.prefix.Length] = '-';
            state.number.TryFormat(span[(state.prefix.Length + 1)..], out _, "D5");
        });
    }

    public static int IndexOfForbidden(ReadOnlySpan<char> text) => text.IndexOfAny(Forbidden);

    public static int IndexOfKeyword(ReadOnlySpan<char> text) => text.IndexOfAny(Keywords);
}
