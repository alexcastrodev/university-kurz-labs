using System.Numerics;

namespace GenericMathLab;

public static class Algorithms
{
    public static T Sum<T>(IEnumerable<T> values)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        T total = T.AdditiveIdentity;
        foreach (var v in values) total += v;
        return total;
    }

    public static T Average<T>(IReadOnlyList<T> values) where T : INumber<T>
        => Sum(values) / T.CreateChecked(values.Count);

    public static TResult AverageAs<T, TResult>(IReadOnlyList<T> values)
        where T : INumber<T> where TResult : INumber<TResult>
    {
        TResult total = TResult.Zero;
        foreach (var v in values) total += TResult.CreateChecked(v);
        return total / TResult.CreateChecked(values.Count);
    }

    public static T Clamp01<T>(T value) where T : INumber<T>
        => T.Clamp(value, T.Zero, T.One);

    public static T Bits<T>(T value) where T : IBinaryInteger<T>
        => T.PopCount(value);

    public static T SumChecked<T>(IEnumerable<T> values)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        T total = T.AdditiveIdentity;
        foreach (var v in values) total = checked(total + v);
        return total;
    }
}
