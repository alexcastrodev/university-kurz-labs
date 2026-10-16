using System.Reflection;
using PrimaryCtorLab;

namespace PrimaryCtorLab.Tests;

public class PrimaryCtorTests
{
    private sealed class FakeRepo : IRepo
    {
        public string Load() => "data";
    }

    [Fact]
    public void AccountShouldHaveOneSourceOfTruthAfterRename()
    {
        var account = new Account("ana");

        account.Rename("bia");

        Assert.Equal("bia", account.Owner);
        Assert.Equal("owner is bia", account.Describe());
    }

    [Fact]
    public void ServiceShouldRejectANullRepoAtConstruction()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new OrderService(null!));

        Assert.Equal("repo", ex.ParamName);
    }

    [Fact]
    public void ServiceShouldKeepTheRepoInAReadonlyField()
    {
        var service = new OrderService(new FakeRepo());

        Assert.Equal("data", service.Load());
        var field = typeof(OrderService).GetField("_repo", BindingFlags.NonPublic | BindingFlags.Instance)!;
        Assert.True(field.IsInitOnly);
    }

    [Fact]
    public void StructShouldCaptureAndMutateItsParameter()
    {
        var counter = new Counter(10);

        counter.Increment();
        counter.Increment();

        Assert.Equal(12, counter.Value);
    }

    [Fact]
    public void DefaultStructHasZeroedParameters() =>
        Assert.Equal(0, default(Counter).Value);

    [Fact]
    public void ParameterlessStructConstructorShouldChainToThePrimaryOne() =>
        Assert.Equal(1, new Counter().Value);

    [Fact]
    public void DerivedClassShouldReadTheInheritedProperties()
    {
        var savings = new Savings("A-1", "ana");

        savings.Owner = "changed";

        Assert.Equal("A-1/changed", savings.ToString());
    }
}
