using System.Reflection;
using PrimaryCtorLab;

namespace PrimaryCtorLab.Tests;

public class BonusTests
{
    [Fact]
    public void RecordShouldGeneratePropertiesAndValueEquality()
    {
        Assert.Equal(1, new Coordinates(1, 2).X);
        Assert.Equal(new Coordinates(1, 2), new Coordinates(1, 2));
        Assert.Equal(3, new Coordinates(1, 2).Sum);
    }

    [Fact]
    public void ServiceShouldTrimAndKeepAReadonlyFieldNamedLikeTheParameter()
    {
        Assert.Equal("Ana", new Service("  Ana ").Show());
        var field = typeof(Service).GetField("name", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.True(field.IsInitOnly);
    }
}
