using System.Globalization;

namespace FormatLab;

public readonly record struct Money(decimal Amount, string Currency) : IFormattable, ISpanFormattable
{
    public string ToString(string? format, IFormatProvider? provider)
    {
        var culture = provider ?? CultureInfo.CurrentCulture;
        return format switch
        {
            null or "" or "short" => $"{Amount.ToString("0.##", culture)}{Currency[0]}",
            "long" => $"{Amount.ToString("N2", culture)} {Currency}",
            _ => throw new FormatException($"Unknown format '{format}'."),
        };
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var text = ToString(format.ToString(), provider);
        charsWritten = 0;
        if (text.Length > destination.Length)
        {
            return false;
        }

        text.CopyTo(destination);
        charsWritten = text.Length;
        return true;
    }
}
