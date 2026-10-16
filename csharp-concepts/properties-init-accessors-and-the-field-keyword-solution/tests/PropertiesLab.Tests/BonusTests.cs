using System.Reflection;
using PropertiesLab;

namespace PropertiesLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldNormalizeTheAmountInTheInitAccessor() =>
        Assert.Equal(2.35m, new Price { Amount = 2.345m }.Amount);

    [Fact]
    public void ShouldMarkTheBackingFieldNonSerialized()
    {
        var backingField = typeof(Session).GetField("<Token>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(backingField);
        Assert.NotNull(backingField.GetCustomAttribute<NonSerializedAttribute>());
    }
}
