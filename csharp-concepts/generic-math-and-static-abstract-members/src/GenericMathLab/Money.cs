using System.Numerics;

namespace GenericMathLab;

// Not a number: no division, no comparison. It only offers what Sum needs.
public readonly record struct Money(long Cents)
    : IAdditionOperators<Money, Money, Money>, IAdditiveIdentity<Money, Money>
{
    public static Money operator +(Money a, Money b)
    {
        // TODO-01: Add the cents of both values, in an unchecked context (plain `+` wraps on overflow).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Money AdditiveIdentity
    {
        get
        {
            // TODO-01: The zero for `+`: the value that changes nothing when added.
            throw new NotImplementedException("Not implemented yet.");
        }
    }
}

// Same capabilities as Money, but it also honors `checked`.
public readonly record struct SafeMoney(long Cents)
    : IAdditionOperators<SafeMoney, SafeMoney, SafeMoney>, IAdditiveIdentity<SafeMoney, SafeMoney>
{
    public static SafeMoney operator +(SafeMoney a, SafeMoney b) => new(unchecked(a.Cents + b.Cents));

    public static SafeMoney operator checked +(SafeMoney a, SafeMoney b)
    {
        // TODO-05: Throw OverflowException on overflow, using a `checked` context for the addition.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static SafeMoney AdditiveIdentity => new(0);
}
