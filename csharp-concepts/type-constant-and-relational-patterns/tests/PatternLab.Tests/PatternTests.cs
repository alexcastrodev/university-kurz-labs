using System.Runtime.CompilerServices;

namespace PatternLab.Tests;

public class PatternTests
{
    [Theory]
    [InlineData(null, "nothing")]
    [InlineData(-5, "negative")]
    [InlineData(0, "zero")]
    [InlineData(7, "positive")]
    [InlineData("", "empty text")]
    [InlineData("hi", "text")]
    [InlineData(true, "yes")]
    [InlineData(false, "no")]
    [InlineData(3.5, "other")]
    public void ShouldDescribeByTypeConstantAndRelationalPatterns(object? value, string expected) =>
        Assert.Equal(expected, Patterns.Describe(value));

    [Theory]
    [InlineData(100, "A")]
    [InlineData(90, "A")]
    [InlineData(89, "B")]
    [InlineData(80, "B")]
    [InlineData(70, "C")]
    [InlineData(69, "F")]
    [InlineData(0, "F")]
    public void ShouldGradeUsingRanges(int score, string expected) =>
        Assert.Equal(expected, Patterns.Grade(score));

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ShouldRejectScoresOutsideTheScale(int score) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Patterns.Grade(score));

    [Theory]
    [InlineData(DayOfWeek.Monday, true)]
    [InlineData(DayOfWeek.Friday, true)]
    [InlineData(DayOfWeek.Saturday, false)]
    [InlineData(DayOfWeek.Sunday, false)]
    public void ShouldDetectWeekdays(DayOfWeek day, bool expected) =>
        Assert.Equal(expected, Patterns.IsWeekday(day));

    [Theory]
    [InlineData('a', true)]
    [InlineData('z', true)]
    [InlineData('A', true)]
    [InlineData('Z', true)]
    [InlineData('0', false)]
    [InlineData('_', false)]
    [InlineData('é', false)]
    public void ShouldDetectAsciiLetters(char c, bool expected) =>
        Assert.Equal(expected, Patterns.IsAsciiLetter(c));

    [Theory]
    [InlineData(0, false, 0)]
    [InlineData(2, true, 0)]
    [InlineData(3, false, 5)]
    [InlineData(12, true, 5)]
    [InlineData(13, true, 8)]
    [InlineData(40, false, 10)]
    [InlineData(64, true, 8)]
    [InlineData(65, false, 7)]
    [InlineData(90, true, 7)]
    public void ShouldPriceTicketsByAgeThenMembership(int age, bool member, int expected) =>
        Assert.Equal((decimal)expected, Patterns.TicketPrice(age, member));

    [Fact]
    public void ShouldRejectNegativeAges() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Patterns.TicketPrice(-1, false));

    [Theory]
    [InlineData(Light.Red, "Green")]
    [InlineData(Light.Green, "Yellow")]
    [InlineData(Light.Yellow, "Red")]
    public void ShouldCycleTheLights(Light light, string expected) =>
        Assert.Equal(expected, Patterns.Next(light));

    [Fact]
    public void ShouldThrowForAnUndefinedEnumValue() =>
        Assert.Throws<SwitchExpressionException>(() => Patterns.Next((Light)99));
}
