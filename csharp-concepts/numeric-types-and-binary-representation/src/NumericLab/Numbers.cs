namespace NumericLab;

public static class Numbers
{
    public static bool Near(double a, double b, double relative = 1e-9, double absolute = 1e-12)
    {
        // TODO-04: Tolerance comparison that mixes an absolute part (values near zero) and a
        //   relative part (large values): |a - b| <= max(absolute, relative * max(|a|, |b|)).
        //   Do not use double.Epsilon. NaN must never be near anything, including NaN.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Scale(decimal value)
    {
        // TODO-05: Return the decimal scale (digits after the point, trailing zeros included):
        //   1m -> 0, 1.0m -> 1, 1.250m -> 3. Read it from decimal.GetBits (bits 16..23 of the
        //   last element) or from decimal.GetBits's documented layout.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static long ToCents(decimal price)
    {
        // TODO-05: Convert a price to whole cents exactly (19.99m -> 1999). Throw
        //   ArgumentException when the price has fractions of a cent (1.999m), instead of
        //   silently truncating.
        throw new NotImplementedException("Not implemented yet.");
    }
}
