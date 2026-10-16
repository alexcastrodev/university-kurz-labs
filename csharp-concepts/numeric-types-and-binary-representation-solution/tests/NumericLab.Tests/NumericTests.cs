using System.Globalization;

namespace NumericLab.Tests;

public class NumericTests
{
    // TODO-00
    [Fact]
    public void ShouldPrintTwosComplementBits()
    {
        Assert.Equal("00000000000000000000000000000101", Bits.ToBinary32(5));
        Assert.Equal("11111111111111111111111111111011", Bits.ToBinary32(-5));
        Assert.Equal("10000000000000000000000000000000", Bits.ToBinary32(int.MinValue));
    }

    [Theory]
    [InlineData(5, -5)]
    [InlineData(-6, 6)]
    [InlineData(0, 0)]
    [InlineData(int.MaxValue, -int.MaxValue)]
    public void ShouldNegateByInvertingAndAddingOne(int value, int expected)
    {
        Assert.Equal(expected, Bits.Negate(value));
    }

    [Fact]
    public void ShouldLeaveMinValueUnchangedWhenNegating()
    {
        Assert.Equal(int.MinValue, Bits.Negate(int.MinValue));
    }

    // TODO-01
    [Fact]
    public void ShouldCopyTheSignBitOnArithmeticShift()
    {
        Assert.Equal(-4, Bits.ShiftRightArithmetic(-16, 2));
        Assert.Equal(-1, Bits.ShiftRightArithmetic(-1, 10));
        Assert.Equal(4, Bits.ShiftRightArithmetic(16, 2));
    }

    [Fact]
    public void ShouldFillWithZerosOnLogicalShift()
    {
        Assert.Equal(1073741820, Bits.ShiftRightLogical(-16, 2));
        Assert.Equal(int.MaxValue, Bits.ShiftRightLogical(-1, 1));
        Assert.Equal(4, Bits.ShiftRightLogical(16, 2));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(31, int.MinValue)]
    [InlineData(32, 1)]
    [InlineData(33, 2)]
    public void ShouldWrapTheShiftCount(int count, int expected)
    {
        Assert.Equal(expected, Bits.OneShiftedLeft(count));
    }

    // TODO-02
    [Fact]
    public void ShouldPackAndUnpackChannels()
    {
        var packed = Bits.PackRgb(255, 128, 1);

        Assert.Equal(0x00FF8001u, packed);
        Assert.Equal(255, Bits.Channel(packed, 0));
        Assert.Equal(128, Bits.Channel(packed, 1));
        Assert.Equal(1, Bits.Channel(packed, 2));
    }

    [Fact]
    public void ShouldRejectAnUnknownChannel()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Bits.Channel(0u, 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => Bits.Channel(0u, -1));
    }

    [Fact]
    public void ShouldSetClearAndTestFlags()
    {
        uint flags = 0;
        flags = Bits.SetFlag(flags, 2);
        flags = Bits.SetFlag(flags, 0);
        Assert.Equal(0b101u, flags);
        Assert.True(Bits.HasFlag(flags, 2));
        Assert.False(Bits.HasFlag(flags, 1));

        flags = Bits.ClearFlag(flags, 2);
        Assert.Equal(1u, flags);
        Assert.Equal(1u, Bits.ClearFlag(flags, 5));
    }

    [Fact]
    public void ShouldHandleTheTopFlagBit()
    {
        var flags = Bits.SetFlag(0u, 31);

        Assert.Equal(0x80000000u, flags);
        Assert.True(Bits.HasFlag(flags, 31));
    }

    // TODO-03
    [Theory]
    [InlineData(0u, 0)]
    [InlineData(255u, 8)]
    [InlineData(0x80000001u, 2)]
    [InlineData(uint.MaxValue, 32)]
    public void ShouldCountSetBits(uint value, int expected)
    {
        Assert.Equal(expected, Bits.CountSetBits(value));
    }

    [Theory]
    [InlineData(0u, 1u)]
    [InlineData(1u, 1u)]
    [InlineData(64u, 64u)]
    [InlineData(65u, 128u)]
    [InlineData(1000u, 1024u)]
    public void ShouldRoundUpToAPowerOfTwo(uint value, uint expected)
    {
        Assert.Equal(expected, Bits.NextPowerOfTwo(value));
    }

    // TODO-04
    [Fact]
    public void ShouldShowThatDoubleEqualityFails()
    {
        // Not a TODO: documents the problem that Near solves.
        Assert.NotEqual(0.3, 0.1 + 0.2);
    }

    [Fact]
    public void ShouldTreatClassicRoundingErrorAsNear()
    {
        Assert.True(Numbers.Near(0.1 + 0.2, 0.3));
        Assert.True(Numbers.Near(1e10 + 1e-3, 1e10));
    }

    [Fact]
    public void ShouldUseTheAbsolutePartNearZero()
    {
        Assert.True(Numbers.Near(0.0, 1e-15));
        Assert.False(Numbers.Near(0.0, 1e-9));
    }

    [Fact]
    public void ShouldUseTheRelativePartForLargeValues()
    {
        Assert.False(Numbers.Near(1e10, 1e10 + 100));
        Assert.True(Numbers.Near(1e10, 1e10 + 1));
    }

    [Fact]
    public void ShouldNeverBeNearNaNOrMismatchedInfinity()
    {
        Assert.False(Numbers.Near(double.NaN, double.NaN));
        Assert.False(Numbers.Near(double.NaN, 1.0));
        Assert.False(Numbers.Near(double.PositiveInfinity, double.NegativeInfinity));
        Assert.False(Numbers.Near(double.PositiveInfinity, 1e300));
    }

    // TODO-05
    [Fact]
    public void ShouldReadTheDecimalScale()
    {
        Assert.Equal(0, Numbers.Scale(1m));
        Assert.Equal(1, Numbers.Scale(1.0m));
        Assert.Equal(2, Numbers.Scale(1.00m));
        Assert.Equal(3, Numbers.Scale(1.250m));
        Assert.Equal(1, Numbers.Scale(-0.5m));
    }

    [Fact]
    public void ShouldKeepEqualValuesWithDifferentScales()
    {
        Assert.Equal(1.0m, 1.00m);
        Assert.NotEqual(Numbers.Scale(1.0m), Numbers.Scale(1.00m));
        Assert.Equal("1.00", 1.00m.ToString(CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData("19.99", 1999)]
    [InlineData("5", 500)]
    [InlineData("1.50", 150)]
    [InlineData("1.500", 150)]
    [InlineData("0.01", 1)]
    [InlineData("-2.30", -230)]
    public void ShouldConvertPricesToExactCents(string text, long expected)
    {
        var price = decimal.Parse(text, CultureInfo.InvariantCulture);

        Assert.Equal(expected, Numbers.ToCents(price));
    }

    [Fact]
    public void ShouldRejectFractionsOfACent()
    {
        Assert.Throws<ArgumentException>(() => Numbers.ToCents(1.999m));
    }

    [Fact]
    public void ShouldSumTenthsExactlyWithDecimal()
    {
        var total = 0m;
        for (var i = 0; i < 1000; i++) total += 0.10m;

        Assert.Equal(100m, total);
        Assert.Equal(10000, Numbers.ToCents(total));
    }
}
