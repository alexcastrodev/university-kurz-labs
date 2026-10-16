using System.Numerics;

namespace NumericLab;

public static class Conversions
{
    public static byte ToPixel(double value) =>
        double.IsNaN(value)
            ? (byte)0
            : (byte)Math.Clamp(Math.Round(value, MidpointRounding.AwayFromZero), byte.MinValue, byte.MaxValue);

    public static decimal RoundToCents(decimal amount) =>
        Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public static int ToIndex(double value) => checked((int)Math.Truncate(value));

    public static int ToIntWithConvert(double value) => Convert.ToInt32(value);

    public static int SumExact(IEnumerable<int> values)
    {
        var total = 0;
        foreach (var value in values)
        {
            total = checked(total + value);
        }

        return total;
    }

    public static bool TryAddExact(int first, int second, out int result)
    {
        var sum = (long)first + second;
        if (sum is < int.MinValue or > int.MaxValue)
        {
            result = 0;
            return false;
        }

        result = (int)sum;
        return true;
    }

    // TODO-06 (optional)
    public static TTo NarrowChecked<TFrom, TTo>(TFrom value)
        where TFrom : INumberBase<TFrom>
        where TTo : INumberBase<TTo> =>
        TTo.CreateChecked(value);

    // TODO-07 (optional)
    public static int AddSaturating(int first, int second) =>
        int.CreateSaturating((long)first + second);

    // TODO-08 (optional)
    public static IReadOnlyList<decimal> SplitEvenly(decimal total, int parts)
    {
        var share = Math.Round(total / parts, 2, MidpointRounding.ToZero);
        var shares = Enumerable.Repeat(share, parts).ToArray();
        shares[^1] += total - share * parts;
        return shares;
    }
}
