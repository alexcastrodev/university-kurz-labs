using PropertiesLab;

namespace PropertiesLab.Tests;

public class PropertiesTests
{
    [Fact]
    public void ShouldStoreAValidTemperature() =>
        Assert.Equal(21.5, new Temperature { Celsius = 21.5 }.Celsius);

    [Fact]
    public void ShouldAcceptAbsoluteZero() =>
        Assert.Equal(-273.15, new Temperature { Celsius = -273.15 }.Celsius);

    [Fact]
    public void ShouldRejectBelowAbsoluteZero() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Temperature { Celsius = -300 });

    [Fact]
    public void ShouldKeepThePreviousValueWhenTheSetterRejects()
    {
        var temperature = new Temperature { Celsius = 10 };

        Assert.Throws<ArgumentOutOfRangeException>(() => temperature.Celsius = -500);

        Assert.Equal(10, temperature.Celsius);
    }

    [Fact]
    public void ShouldTrimTheName() =>
        Assert.Equal("Ana", new Person { Name = "  Ana " }.Name);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldRejectABlankName(string? name) =>
        Assert.ThrowsAny<ArgumentException>(() => new Person { Name = name! });

    [Fact]
    public void ShouldDefaultTheTitleLazily() =>
        Assert.Equal("untitled", new Document().Title);

    [Fact]
    public void ShouldReturnTheAssignedTitle() =>
        Assert.Equal("Report", new Document { Title = "Report" }.Title);

    [Fact]
    public void ShouldRaiseChangedWithTheNewValue()
    {
        var counter = new Counter();
        var seen = new List<int>();
        counter.Changed += seen.Add;

        counter.Value = 5;
        counter.Value = 7;

        Assert.Equal([5, 7], seen);
    }

    [Fact]
    public void ShouldNotRaiseChangedWhenTheValueIsTheSame()
    {
        var counter = new Counter { Value = 3 };
        var raised = 0;
        counter.Changed += _ => raised++;

        counter.Value = 3;

        Assert.Equal(0, raised);
    }
}
