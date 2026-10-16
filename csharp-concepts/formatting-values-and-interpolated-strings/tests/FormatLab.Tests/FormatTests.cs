using System.Globalization;

namespace FormatLab.Tests;

public class FormatTests
{
    private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");
    private static readonly CultureInfo DeDe = CultureInfo.GetCultureInfo("de-DE");

    private static void WithCurrentCulture(CultureInfo culture, Action action)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = culture;
        try
        {
            action();
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void ShouldAlignNameLeftAndPriceRightWithUsFormatting() =>
        Assert.Equal("Tea       " + "  1,234.50", Formatter.Row("Tea", 1234.5m, EnUs));

    [Fact]
    public void ShouldFollowTheGivenCultureForSeparators()
    {
        Assert.Equal("Tea       " + "  1.234,50", Formatter.Row("Tea", 1234.5m, PtBr));
        Assert.Equal("Tea       " + "  1.234,50", Formatter.Row("Tea", 1234.5m, DeDe));
    }

    [Fact]
    public void ShouldKeepTheCacheKeyInvariantWhateverTheMachineCulture()
    {
        WithCurrentCulture(PtBr, () => Assert.Equal("tea:1.5", Formatter.CacheKey("tea", 1.5m)));
        WithCurrentCulture(DeDe, () => Assert.Equal("tea:1.5", Formatter.CacheKey("tea", 1.5m)));
    }

    [Theory]
    [InlineData(0.5, "0.5")]
    [InlineData(1234.567, "1234.57")]
    [InlineData(2.0, "2")]
    [InlineData(-0.5, "-0.5")]
    public void ShouldTrimTrailingZerosWithoutCultureInfluence(double value, string expected)
    {
        WithCurrentCulture(PtBr, () => Assert.Equal(expected, Formatter.Compact(value)));
    }

    [Fact]
    public void ShouldShowZeroAsZeroNotAsAnEmptyString() =>
        Assert.Equal("0", Formatter.Compact(0.0));

    [Fact]
    public void ShouldFormatMoneyShortByDefault()
    {
        var money = new Money(12.5m, "EUR");

        Assert.Equal("12.5E", money.ToString(null, EnUs));
        Assert.Equal("12.5E", money.ToString("", EnUs));
        Assert.Equal("12.5E", money.ToString("short", EnUs));
    }

    [Fact]
    public void ShouldFormatMoneyLongWithTheProviderCulture()
    {
        var money = new Money(1234.5m, "EUR");

        Assert.Equal("1,234.50 EUR", money.ToString("long", EnUs));
        Assert.Equal("1.234,50 EUR", money.ToString("long", PtBr));
    }

    [Fact]
    public void ShouldWorkInsideAnInterpolatedStringWithAlignment()
    {
        var money = new Money(12.5m, "EUR");

        Assert.Equal("[   12.50 EUR]", string.Create(EnUs, $"[{money,12:long}]"));
    }

    [Fact]
    public void ShouldRejectUnknownMoneyFormats() =>
        Assert.Throws<FormatException>(() => new Money(1m, "EUR").ToString("verbose", EnUs));

    [Fact]
    public void ShouldWriteALineWhenTheLoggerIsEnabled()
    {
        var logger = new Logger();
        var who = "Ana";
        var id = 42;

        logger.Debug($"hello {who} id={id,5:D3} done");

        Assert.Equal(["hello Ana id=  042 done"], logger.Lines);
    }

    [Fact]
    public void ShouldNotEvaluateTheHolesWhenTheLoggerIsDisabled()
    {
        var logger = new Logger { Enabled = false };
        var calls = 0;
        int Expensive()
        {
            calls++;
            return 7;
        }

        logger.Debug($"skipped {Expensive()}");

        Assert.Equal(0, calls);
        Assert.Empty(logger.Lines);
    }
}
