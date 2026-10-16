namespace NullLab.Tests;

public class NullTests
{
    [Fact]
    public void ShouldRenameAndSkipTheRightSideForANullCustomer()
    {
        var calls = 0;
        string NewName()
        {
            calls++;
            return "Bia";
        }
        var ana = new Customer { Name = "Ana" };

        Operations.Rename(null, NewName);
        Assert.Equal(0, calls);

        Operations.Rename(ana, NewName);
        Assert.Equal("Bia", ana.Name);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void ShouldAddAYearAndReturnTheNewAge()
    {
        var ana = new Customer { Name = "Ana", Age = 41 };

        Assert.Equal(42, Operations.Birthday(ana));
        Assert.Equal(42, ana.Age);
    }

    [Fact]
    public void ShouldReturnNullWhenThereIsNoBirthdayCustomer() =>
        Assert.Null(Operations.Birthday(null));

    [Fact]
    public void ShouldSetTheNicknameOnlyWhenItIsStillNull()
    {
        var ana = new Customer { Name = "Ana" };

        Operations.EnsureNickname(ana, "Aninha");
        Operations.EnsureNickname(ana, "ignored");
        Operations.EnsureNickname(null, "nobody");

        Assert.Equal("Aninha", ana.Nickname);
    }

    [Fact]
    public void ShouldSetTheCustomerNameThroughTheOrder()
    {
        var order = new Order { Customer = new Customer { Name = "Ana" } };

        Operations.SetCustomerName(order, "Bia");

        Assert.Equal("Bia", order.Customer.Name);
    }

    [Fact]
    public void ShouldStopTheChainAtTheFirstNull()
    {
        Operations.SetCustomerName(null, "Bia");
        var order = new Order { Customer = null };

        Operations.SetCustomerName(order, "Bia");

        Assert.Null(order.Customer);
    }

    [Fact]
    public void ShouldSubscribeToTheClickedEvent()
    {
        var button = new Button();
        var clicks = 0;

        Operations.Subscribe(button, () => clicks++);
        Operations.Subscribe(null, () => clicks += 100);
        button.Click();

        Assert.Equal(1, clicks);
    }

    [Fact]
    public void ShouldAssignThroughTheIndexerAndSkipTheRightSideForNull()
    {
        var calls = 0;
        int Value()
        {
            calls++;
            return 7;
        }
        var map = new Dictionary<string, int>();

        Operations.Put(null, "a", Value);
        Assert.Equal(0, calls);

        Operations.Put(map, "a", Value);
        Assert.Equal(7, map["a"]);
        Assert.Equal(1, calls);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(new int[0], true)]
    [InlineData(new[] { 1 }, true)]
    [InlineData(new[] { 1, 2 }, false)]
    public void ShouldTreatANullArrayAsShort(int[]? numbers, bool expected) =>
        Assert.Equal(expected, Operations.IsShort(numbers));

    [Fact]
    public void ShouldAddTenToTheBonusNotToTheFallback()
    {
        Assert.Equal(15, Operations.Total(5));
        Assert.Equal(10, Operations.Total(null));
    }

    [Fact]
    public void ShouldRunBothChecksEvenWhenTheFirstFails()
    {
        var calls = 0;
        bool Check(bool result)
        {
            calls++;
            return result;
        }

        var valid = Operations.BothValid(() => Check(false), () => Check(true));

        Assert.False(valid);
        Assert.Equal(2, calls);
        Assert.True(Operations.BothValid(() => true, () => true));
    }
}
