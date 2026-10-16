namespace RegexLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldCaptureDateGroupsFromThePartialProperty()
    {
        var match = Patterns.DateProp.Match("on 2025-11-11");

        Assert.Equal("2025", match.Groups["y"].Value);
        Assert.Same(Patterns.DateProp, Patterns.DateProp);
    }

    [Fact]
    public void ShouldCountDates()
    {
        Assert.Equal(2, Patterns.CountDates("2025-01-02 and 2025-03-04, not 25-1-1"));
    }
}
