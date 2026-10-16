using NumericLab;

namespace NumericLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldNarrowWhenTheValueFits() =>
        Assert.Equal((byte)200, Conversions.NarrowChecked<int, byte>(200));

    [Fact]
    public void ShouldThrowWhenNarrowingDoesNotFit() =>
        Assert.Throws<OverflowException>(() => Conversions.NarrowChecked<int, byte>(300));

    [Theory]
    [InlineData(int.MaxValue, 1, int.MaxValue)]
    [InlineData(int.MinValue, -1, int.MinValue)]
    [InlineData(2, 3, 5)]
    public void ShouldSaturateInsteadOfWrapping(int first, int second, int expected) =>
        Assert.Equal(expected, Conversions.AddSaturating(first, second));

    [Fact]
    public void ShouldSplitWithoutLosingACent()
    {
        var shares = Conversions.SplitEvenly(100m, 3);

        Assert.Equal([33.33m, 33.33m, 33.34m], shares);
        Assert.Equal(100m, shares.Sum());
    }
}
