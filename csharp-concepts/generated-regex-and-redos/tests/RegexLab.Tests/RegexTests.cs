using System.Text.RegularExpressions;

namespace RegexLab.Tests;

public class RegexTests
{
    private static readonly string Hostile = new string('a', 40) + "!";

    [Fact]
    public void ShouldCaptureDateGroups()
    {
        var match = Patterns.Date().Match("released on 2025-11-11.");

        Assert.True(match.Success);
        Assert.Equal("2025", match.Groups["y"].Value);
        Assert.Equal("11", match.Groups["m"].Value);
        Assert.Equal("11", match.Groups["d"].Value);
    }

    [Fact]
    public void ShouldReturnTheSameCachedInstanceForGeneratedRegex()
    {
        Assert.Same(Patterns.Date(), Patterns.Date());
    }

    [Fact]
    public void ShouldUseAGeneratedSubclassForDate()
    {
        Assert.NotEqual(typeof(Regex), Patterns.Date().GetType());
    }

    [Fact]
    public void ShouldKeepTheTimeoutOnTheGeneratedLegacyPattern()
    {
        Assert.NotEqual(typeof(Regex), Patterns.Legacy().GetType());
        Assert.Equal(TimeSpan.FromMilliseconds(200), Patterns.Legacy().MatchTimeout);
    }

    [Fact]
    public void ShouldTimeOutWhenLegacyPatternBacktracksCatastrophically()
    {
        Assert.Throws<RegexMatchTimeoutException>(() => Patterns.Legacy().IsMatch(Hostile));
    }

    [Theory]
    [InlineData("a", true)]
    [InlineData("aaaa", true)]
    [InlineData("", false)]
    [InlineData("aab", false)]
    public void ShouldAcceptTheSameStringsAsLegacyWhenUsingSafe(string input, bool expected)
    {
        Assert.Equal(expected, Patterns.Safe().IsMatch(input));
        Assert.Equal(expected, Patterns.Legacy().IsMatch(input));
    }

    [Fact]
    public void ShouldRejectHostileInputWithoutTimeoutWhenUsingSafe()
    {
        var regex = Patterns.Safe();

        Assert.NotEqual(typeof(Regex), regex.GetType());
        Assert.Equal(TimeSpan.FromMilliseconds(1000), regex.MatchTimeout);
        Assert.False(regex.IsMatch(Hostile));
    }

    [Fact]
    public void ShouldRejectHostileInputWithoutTimeoutWhenUsingNonBacktracking()
    {
        Assert.False(Patterns.Linear().IsMatch(Hostile));
        Assert.True(Patterns.Linear().IsMatch("aaaa"));
    }

    [Fact]
    public void ShouldFallBackToAPlainRegexForNonBacktracking()
    {
        Assert.Equal(typeof(Regex), Patterns.Linear().GetType());
    }

    [Fact]
    public void ShouldReturnTooLongWithoutRunningTheRegex()
    {
        Assert.Equal(ValidationResult.TooLong, InputValidator.Check(new string('a', InputValidator.MaxLength + 1)));
    }

    [Fact]
    public void ShouldReturnTimedOutForHostileInput()
    {
        Assert.Equal(ValidationResult.TimedOut, InputValidator.Check(Hostile));
    }

    [Fact]
    public void ShouldReturnValidAndInvalidForNormalInput()
    {
        Assert.Equal(ValidationResult.Valid, InputValidator.Check("aaaa"));
        Assert.Equal(ValidationResult.Invalid, InputValidator.Check("b"));
    }
}
