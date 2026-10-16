using ExtensionLab;

namespace ExtensionLab.Tests;

public class BonusTests
{
    [Fact]
    public void PlusShouldConcatenateSequences()
    {
        List<string> names = ["ana", "bia"];

        Assert.Equal(["ana", "bia", "caio"], names + ["caio"]);
    }

    [Fact]
    public void MoneyShouldSupportAnExtensionOperator() =>
        Assert.Equal(new Money(30), new Money(10) * 3);

    [Fact]
    public void MathShouldGainAStaticExtensionMethod() =>
        Assert.Equal(50, Math.Remap(5, 0, 10, 0, 100));
}
