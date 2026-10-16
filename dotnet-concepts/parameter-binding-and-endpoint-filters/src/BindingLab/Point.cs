using System.Globalization;

namespace BindingLab;

public readonly record struct Point(double X, double Y)
{
    public static bool TryParse(string? s, IFormatProvider? provider, out Point result)
    {
        result = default;
        // TODO-02: Parse "x,y" (two doubles separated by a comma). Return false when the
        // text is missing or malformed. Use the invariant culture so "12.3" works on any server.
        return false;
    }
}
