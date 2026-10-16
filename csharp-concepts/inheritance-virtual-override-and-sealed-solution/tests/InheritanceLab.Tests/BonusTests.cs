using InheritanceLab;

namespace InheritanceLab.Tests;

public class BonusTests
{
    [Fact]
    public void CircleAreaShouldBeASealedOverride()
    {
        var area = typeof(Circle).GetMethod(nameof(Circle.Area))!;

        Assert.True(area.IsFinal);
        Assert.True(area.IsVirtual);
    }

    [Fact]
    public void SquareAreaShouldStayOverridable() =>
        Assert.False(typeof(Square).GetMethod(nameof(Square.Area))!.IsFinal);
}
