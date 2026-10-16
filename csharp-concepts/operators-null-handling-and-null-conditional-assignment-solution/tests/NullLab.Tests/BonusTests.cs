namespace NullLab.Tests;

public class BonusTests
{
    [Fact]
    public void ShouldEvaluateTheReceiverOnlyOnce()
    {
        var ana = new Customer { Name = "Ana", Age = 30 };
        var calls = 0;
        Customer? Get()
        {
            calls++;
            return ana;
        }

        Operations.BirthdayOf(Get);

        Assert.Equal(40, ana.Age);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void ShouldReportWhetherTheAssignmentHappened()
    {
        var ana = new Customer { Name = "Ana" };

        Assert.True(Operations.TryRename(ana, "Bia"));
        Assert.Equal("Bia", ana.Name);
        Assert.False(Operations.TryRename(null, "Bia"));
    }
}
