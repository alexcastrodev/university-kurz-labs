using System.Globalization;

namespace BindingLab;

public readonly record struct Point(double X, double Y)
{
    public static bool TryParse(string? s, IFormatProvider? provider, out Point result)
    {
        result = default;
        var parts = s?.Split(',');
        if (parts is not { Length: 2 }) return false;
        if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var x)) return false;
        if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var y)) return false;
        result = new Point(x, y);
        return true;
    }
}
