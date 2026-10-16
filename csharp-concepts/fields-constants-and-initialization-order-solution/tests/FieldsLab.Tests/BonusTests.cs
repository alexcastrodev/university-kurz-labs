using FieldsLab;

namespace FieldsLab.Tests;

public class BonusTests
{
    [Fact]
    public void EachClosedGenericTypeShouldHaveItsOwnStatics()
    {
        Assert.Equal(1, Counter<int>.Next());
        Assert.Equal(2, Counter<int>.Next());
        Assert.Equal(1, Counter<string>.Next());
    }

    [Fact]
    public void ParameterlessConstructorShouldChainToTheFullOne()
    {
        var settings = new Settings();

        Assert.Equal(("localhost", 5432), (settings.Host, settings.Port));
    }

    [Fact]
    public void HostConstructorShouldKeepTheDefaultPort() =>
        Assert.Equal(5432, new Settings("db.internal").Port);

    [Fact]
    public void FullConstructorShouldValidate()
    {
        Assert.Throws<ArgumentException>(() => new Settings(" ", 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Settings("h", 0));
    }
}
