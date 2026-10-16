using System.Globalization;
using NumericLab;

namespace NumericLab.Tests;

public class ConversionTests
{
    [Theory]
    [InlineData(2.5, 3)]
    [InlineData(2.4, 2)]
    [InlineData(-10.0, 0)]
    [InlineData(300.7, 255)]
    [InlineData(double.NaN, 0)]
    public void ShouldClampAndRoundPixels(double input, byte expected) =>
        Assert.Equal(expected, Conversions.ToPixel(input));

    [Theory]
    [InlineData("2.345", "2.35")]
    [InlineData("2.355", "2.36")]
    [InlineData("-2.345", "-2.35")]
    [InlineData("2.344", "2.34")]
    public void ShouldRoundHalvesAwayFromZero(string input, string expected) =>
        Assert.Equal(Parse(expected), Conversions.RoundToCents(Parse(input)));

    private static decimal Parse(string text) => decimal.Parse(text, CultureInfo.InvariantCulture);

    [Theory]
    [InlineData(2.9, 2)]
    [InlineData(-2.9, -2)]
    [InlineData(0.99, 0)]
    public void ShouldTruncateTowardZeroForIndexes(double input, int expected) =>
        Assert.Equal(expected, Conversions.ToIndex(input));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(3_000_000_000.0)]
    [InlineData(-3_000_000_000.0)]
    public void ShouldThrowForIndexesThatDoNotFit(double input) =>
        Assert.Throws<OverflowException>(() => Conversions.ToIndex(input));

    [Theory]
    [InlineData(2.5, 2)]
    [InlineData(3.5, 4)]
    [InlineData(2.6, 3)]
    [InlineData(-2.5, -2)]
    public void ShouldRoundHalvesToEvenWithConvert(double input, int expected) =>
        Assert.Equal(expected, Conversions.ToIntWithConvert(input));

    [Fact]
    public void ShouldSumWithoutOverflow() =>
        Assert.Equal(6, Conversions.SumExact([1, 2, 3]));

    [Fact]
    public void ShouldThrowWhenTheSumOverflows() =>
        Assert.Throws<OverflowException>(() => Conversions.SumExact([int.MaxValue, 1]));

    [Fact]
    public void ShouldThrowWhenTheSumUnderflows() =>
        Assert.Throws<OverflowException>(() => Conversions.SumExact([int.MinValue, -1]));

    [Fact]
    public void ShouldAddWhenItFits()
    {
        Assert.True(Conversions.TryAddExact(40, 2, out var result));
        Assert.Equal(42, result);
    }

    [Theory]
    [InlineData(int.MaxValue, 1)]
    [InlineData(int.MinValue, -1)]
    public void ShouldReportFailureWithoutThrowing(int first, int second)
    {
        Assert.False(Conversions.TryAddExact(first, second, out var result));
        Assert.Equal(0, result);
    }
}
