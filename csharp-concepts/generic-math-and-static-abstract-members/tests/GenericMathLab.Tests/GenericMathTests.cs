using System.Numerics;

namespace GenericMathLab.Tests;

public class GenericMathTests
{
    [Fact]
    public void ShouldSumBuiltInNumericTypesWithOneMethod()
    {
        Assert.Equal(6, Algorithms.Sum<int>([1, 2, 3]));
        Assert.Equal(0.75, Algorithms.Sum<double>([0.5, 0.25]));
        Assert.Equal(0.3m, Algorithms.Sum<decimal>([0.1m, 0.2m]));
        Assert.Equal(BigInteger.Pow(2, 100) + 1, Algorithms.Sum<BigInteger>([BigInteger.Pow(2, 100), 1]));
    }

    [Fact]
    public void ShouldReturnTheAdditiveIdentityForAnEmptySequence()
    {
        Assert.Equal(0, Algorithms.Sum<int>([]));
        Assert.Equal(new Money(0), Algorithms.Sum<Money>([]));
    }

    [Fact]
    public void ShouldSumMoneyEvenThoughItIsNotANumber()
    {
        var total = Algorithms.Sum<Money>([new(1050), new(250)]);

        Assert.Equal(new Money(1300), total);
        Assert.Equal(new Money(0), Money.AdditiveIdentity);
        Assert.Equal(new Money(7), new Money(3) + new Money(4));
    }

    [Fact]
    public void ShouldWrapOnOverflowInAPlainSum()
    {
        Assert.Equal(int.MinValue, Algorithms.Sum<int>([int.MaxValue, 1]));
    }

    [Fact]
    public void ShouldTruncateWhenAveragingIntegersInTheSameType()
    {
        Assert.Equal(1, Algorithms.Average<int>([1, 2]));
        Assert.Equal(1.5, Algorithms.Average<double>([1, 2]));
    }

    [Fact]
    public void ShouldAverageIntoAnotherResultType()
    {
        Assert.Equal(1.5, Algorithms.AverageAs<int, double>([1, 2]));
        Assert.Equal(2.5m, Algorithms.AverageAs<int, decimal>([2, 3]));
    }

    [Fact]
    public void ShouldNotOverflowWhenAccumulatingInAWiderType()
    {
        Assert.Equal((long)int.MaxValue, Algorithms.AverageAs<int, long>([int.MaxValue, int.MaxValue]));
    }

    [Fact]
    public void ShouldThrowWhenTheAverageDoesNotFitTheResultType()
    {
        Assert.Throws<OverflowException>(() => Algorithms.AverageAs<long, int>([long.MaxValue]));
    }

    [Fact]
    public void ShouldClampToZeroAndOneForAnyNumber()
    {
        Assert.Equal(1.0, Algorithms.Clamp01(1.7));
        Assert.Equal(0f, Algorithms.Clamp01(-2f));
        Assert.Equal(0.5m, Algorithms.Clamp01(0.5m));
        Assert.Equal(1, Algorithms.Clamp01(42));
    }

    [Fact]
    public void ShouldCountBitsForEveryIntegerWidth()
    {
        Assert.Equal(8, Algorithms.Bits(255));
        Assert.Equal(8UL, Algorithms.Bits(255UL));
        Assert.Equal((byte)3, Algorithms.Bits((byte)7));
    }

    [Fact]
    public void ShouldThrowInACheckedSumOfIntegers()
    {
        Assert.Equal(6, Algorithms.SumChecked<int>([1, 2, 3]));
        Assert.Throws<OverflowException>(() => Algorithms.SumChecked<int>([int.MaxValue, 1]));
    }

    [Fact]
    public void ShouldKeepIeeeSemanticsForDoublesEvenWhenChecked()
    {
        Assert.Equal(double.PositiveInfinity, Algorithms.SumChecked<double>([double.MaxValue, double.MaxValue]));
    }

    [Fact]
    public void ShouldOnlyProtectTypesThatDefineACheckedOperator()
    {
        var unsafeSum = Algorithms.SumChecked<Money>([new(long.MaxValue), new(1)]);
        Assert.Equal(new Money(long.MinValue), unsafeSum);

        Assert.Throws<OverflowException>(() => Algorithms.SumChecked<SafeMoney>([new(long.MaxValue), new(1)]));
        Assert.Equal(new SafeMoney(3), Algorithms.SumChecked<SafeMoney>([new(1), new(2)]));
    }
}
