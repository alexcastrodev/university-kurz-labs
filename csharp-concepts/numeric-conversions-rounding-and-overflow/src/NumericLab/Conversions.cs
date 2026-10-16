namespace NumericLab;

public static class Conversions
{
    public static byte ToPixel(double value)
    {
        // TODO-00: Convert a brightness value to a byte. Round half away from zero (2.5 -> 3),
        //   clamp to 0..255 and map NaN to 0. A plain (byte) cast gives unspecified results
        //   outside the range, so clamp before casting.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static decimal RoundToCents(decimal amount)
    {
        // TODO-01: Round to 2 decimal places the way the accounting rule says: halves go away
        //   from zero (2.345 -> 2.35, -2.345 -> -2.35). The default Math.Round uses banker's
        //   rounding, so choose the MidpointRounding mode explicitly.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ToIndex(double value)
    {
        // TODO-02: Truncate toward zero (2.9 -> 2, -2.9 -> -2) and throw OverflowException for
        //   NaN, infinities and values outside the int range. An unchecked cast silently
        //   returns garbage for those, so make the cast checked.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ToIntWithConvert(double value)
    {
        // TODO-03: Use System.Convert, not a cast. Convert.ToInt32 rounds to the nearest integer
        //   and sends halves to the even neighbor (2.5 -> 2, 3.5 -> 4), unlike the truncating cast.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int SumExact(IEnumerable<int> values)
    {
        // TODO-04: Sum the values and throw OverflowException when the total does not fit in an
        //   int. Without `checked`, int addition wraps around silently.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool TryAddExact(int first, int second, out int result)
    {
        // TODO-05: Return true and the sum when it fits in an int, false (and result = 0) when it
        //   does not. Follow the Try pattern: no exception for the expected failure.
        throw new NotImplementedException("Not implemented yet.");
    }
}
