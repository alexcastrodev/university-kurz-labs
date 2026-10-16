namespace RecordLab.Tests;

public class RecordTests
{
    private static Customer Ana() =>
        new("Ana", new Address("1 Main St", "Lisbon"), new List<string> { "vip" });

    [Fact]
    public void ShouldMoveWithoutChangingTheOriginal()
    {
        var ana = Ana();

        var moved = Operations.MoveTo(ana, "Porto");

        Assert.Equal("Porto", moved.Address.City);
        Assert.Equal("1 Main St", moved.Address.Street);
        Assert.Equal("Lisbon", ana.Address.City);
    }

    [Fact]
    public void ShouldAppendATagWithoutMutatingTheOriginalList()
    {
        var ana = Ana();

        var tagged = Operations.AddTag(ana, "newsletter");

        Assert.Equal(["vip", "newsletter"], tagged.Tags);
        Assert.Equal(["vip"], ana.Tags);
        Assert.NotSame(ana.Tags, tagged.Tags);
    }

    [Fact]
    public void ShouldRejectANegativeAmountInTheConstructor() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1m, "EUR"));

    [Fact]
    public void ShouldRejectANegativeAmountInAWithExpression()
    {
        var money = new Money(10m, "EUR");

        Assert.Throws<ArgumentOutOfRangeException>(() => money with { Amount = -5m });
    }

    [Fact]
    public void ShouldKeepValueEqualityOnMoney() =>
        Assert.Equal(new Money(10m, "EUR"), new Money(10m, "EUR"));

    [Fact]
    public void ShouldAddMoneyOfTheSameCurrency()
    {
        var sum = Operations.Add(new Money(10m, "EUR"), new Money(2.5m, "EUR"));

        Assert.Equal(new Money(12.5m, "EUR"), sum);
    }

    [Fact]
    public void ShouldRefuseToAddDifferentCurrencies() =>
        Assert.Throws<InvalidOperationException>(
            () => Operations.Add(new Money(1m, "EUR"), new Money(1m, "USD")));

    [Fact]
    public void ShouldTreatDifferentListInstancesAsDifferentRecords()
    {
        var first = Ana();
        var second = Ana();

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ShouldCompareCustomersByContent()
    {
        Assert.True(Operations.SameContent(Ana(), Ana()));
        Assert.False(Operations.SameContent(Ana(), Operations.AddTag(Ana(), "x")));
        Assert.False(Operations.SameContent(Ana(), Operations.MoveTo(Ana(), "Porto")));
    }
}
