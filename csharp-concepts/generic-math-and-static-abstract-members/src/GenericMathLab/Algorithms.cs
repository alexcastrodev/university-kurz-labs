using System.Numerics;

namespace GenericMathLab;

public static class Algorithms
{
    public static T Sum<T>(IEnumerable<T> values)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        // TODO-00: Start from T.AdditiveIdentity and add every value with `+=`.
        //   The constraint asks for the least: a sum needs `+` and a zero, nothing else.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T Average<T>(IReadOnlyList<T> values) where T : INumber<T>
    {
        // TODO-02: Divide the sum by the count. Convert the count with T.CreateChecked.
        //   Notice what happens for int: the division is an integer division.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static TResult AverageAs<T, TResult>(IReadOnlyList<T> values)
        where T : INumber<T> where TResult : INumber<TResult>
    {
        // TODO-02: Accumulate in TResult (start at TResult.Zero, convert each value with
        //   TResult.CreateChecked) and divide by the count converted to TResult.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T Clamp01<T>(T value) where T : INumber<T>
    {
        // TODO-03: Clamp between T.Zero and T.One.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T Bits<T>(T value) where T : IBinaryInteger<T>
    {
        // TODO-04: Count the set bits, once for every integer width.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T SumChecked<T>(IEnumerable<T> values)
        where T : IAdditionOperators<T, T, T>, IAdditiveIdentity<T, T>
    {
        // TODO-05: Like Sum, but add inside a `checked` context: total = checked(total + v).
        throw new NotImplementedException("Not implemented yet.");
    }
}
