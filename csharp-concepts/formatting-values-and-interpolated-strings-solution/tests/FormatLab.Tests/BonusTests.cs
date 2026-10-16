using System.Globalization;

namespace FormatLab.Tests;

public class BonusTests
{
    private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    [Fact]
    public void ShouldWriteMoneyStraightIntoASpan()
    {
        var money = new Money(12.5m, "EUR");
        Span<char> buffer = stackalloc char[64];

        var ok = buffer.TryWrite(EnUs, $"{money:short} {42}", out var written);

        Assert.True(ok);
        Assert.Equal("12.5E 42", buffer[..written].ToString());
    }

    [Fact]
    public void ShouldReportFalseWhenTheBufferIsTooSmall()
    {
        var money = new Money(12.5m, "EUR");
        Span<char> buffer = stackalloc char[3];

        Assert.False(money.TryFormat(buffer, out var written, "long", EnUs));
        Assert.Equal(0, written);
    }

    [Fact]
    public void ShouldFormatAGreetingFromAParsedTemplate()
    {
        Assert.Equal("Hello Ana, you have 1,200 new messages", Formatter.Greeting("Ana", 1200, EnUs));
        Assert.Equal("Hello Ana, you have 1.200 new messages", Formatter.Greeting("Ana", 1200, PtBr));
    }
}
