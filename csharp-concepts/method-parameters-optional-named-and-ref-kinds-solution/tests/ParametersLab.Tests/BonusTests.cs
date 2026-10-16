using ParametersLab;

namespace ParametersLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldWriteThroughAReturnedReference()
    {
        var values = new[] { 1, 2, 3 };

        Methods.FindSlot(values, 2) = 20;

        Assert.Equal([1, 20, 3], values);
    }

    [Fact]
    public void ShouldThrowWhenTheTargetIsMissing() =>
        Assert.Throws<KeyNotFoundException>(() => Methods.FindSlot([1, 2], 9));

    [Fact]
    public void ShouldCountParsableTextsIgnoringTheValue() =>
        Assert.Equal(2, Methods.CountValid(["1", "x", "42", ""]));
}
