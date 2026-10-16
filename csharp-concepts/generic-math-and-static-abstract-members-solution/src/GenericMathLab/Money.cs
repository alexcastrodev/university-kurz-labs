using System.Numerics;

namespace GenericMathLab;

// Not a number: no division, no comparison. It only offers what Sum needs.
public readonly record struct Money(long Cents)
    : IAdditionOperators<Money, Money, Money>, IAdditiveIdentity<Money, Money>
{
    public static Money operator +(Money a, Money b) => new(unchecked(a.Cents + b.Cents));

    public static Money AdditiveIdentity => new(0);
}

// Same capabilities as Money, but it also honors `checked`.
public readonly record struct SafeMoney(long Cents)
    : IAdditionOperators<SafeMoney, SafeMoney, SafeMoney>, IAdditiveIdentity<SafeMoney, SafeMoney>
{
    public static SafeMoney operator +(SafeMoney a, SafeMoney b) => new(unchecked(a.Cents + b.Cents));

    public static SafeMoney operator checked +(SafeMoney a, SafeMoney b) => new(checked(a.Cents + b.Cents));

    public static SafeMoney AdditiveIdentity => new(0);
}
