namespace NumericLab;

public static class Numbers
{
    public static bool Near(double a, double b, double relative = 1e-9, double absolute = 1e-12)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        if (double.IsInfinity(a) || double.IsInfinity(b)) return a == b;
        return Math.Abs(a - b) <= Math.Max(absolute, relative * Math.Max(Math.Abs(a), Math.Abs(b)));
    }

    public static int Scale(decimal value) => (decimal.GetBits(value)[3] >> 16) & 0xFF;

    public static long ToCents(decimal price)
    {
        var cents = price * 100m;
        if (cents != decimal.Truncate(cents))
            throw new ArgumentException("Price has fractions of a cent.", nameof(price));
        return (long)cents;
    }
}
