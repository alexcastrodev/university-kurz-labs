using System.Runtime.CompilerServices;

namespace GuardLab.Tests;

public class GuardTests
{
    private static Order ValidOrder() => new()
    {
        Customer = new Customer { Email = "ana@example.com" },
        Quantity = 2,
        Discount = 0.1m,
    };

    [Fact]
    public void ShouldPlaceAValidOrder() =>
        Assert.Equal("placed 2", Guards.Place(ValidOrder()));

    [Fact]
    public void ShouldRejectANullOrder()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => Guards.Place(null!));

        Assert.Equal("order", ex.ParamName);
    }

    [Fact]
    public void ShouldRejectAMissingCustomerNamingTheExpression()
    {
        var order = ValidOrder();
        order.Customer = null;

        var ex = Assert.Throws<ArgumentNullException>(() => Guards.Place(order));

        Assert.Equal("order.Customer", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldRejectABlankEmail(string? email)
    {
        var order = ValidOrder();
        order.Customer!.Email = email;

        var ex = Assert.ThrowsAny<ArgumentException>(() => Guards.Place(order));

        Assert.Equal("order.Customer.Email", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ShouldRejectANonPositiveQuantity(int quantity)
    {
        var order = ValidOrder();
        order.Quantity = quantity;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Guards.Place(order));

        Assert.Equal("order.Quantity", ex.ParamName);
    }

    [Fact]
    public void ShouldRejectADiscountAboveFiftyPercent()
    {
        var order = ValidOrder();
        order.Discount = 0.7m;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Guards.Place(order));

        Assert.Equal("order.Discount", ex.ParamName);
        Assert.Equal(0.7m, ex.ActualValue);
    }

    [Fact]
    public void ShouldNameTheCallerArgumentInANotDefaultGuard()
    {
        var id = Guid.Empty;

        var ex = Assert.Throws<ArgumentException>(() => Guards.NotDefault(id));

        Assert.Equal("id", ex.ParamName);
    }

    [Fact]
    public void ShouldRejectNullAndReturnNonDefaultValues()
    {
        string? name = null;
        var other = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() => Guards.NotDefault(name));
        Assert.Equal(other, Guards.NotDefault(other));
    }

    [Fact]
    public void ShouldReturnTheFirstArgumentOrThrow()
    {
        Assert.Equal("a", Guards.FirstArg(["a", "b"]));
        var ex = Assert.Throws<ArgumentException>(() => Guards.FirstArg([]));
        Assert.StartsWith("No arguments.", ex.Message);
    }

    [Fact]
    public void ShouldReadAValidIndex() =>
        Assert.Equal(20, Guards.Get([10, 20, 30], 1));

    [Theory]
    [InlineData(-1)]
    [InlineData(3)]
    public void ShouldThrowFromTheHelperForABadIndex(int index)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => Guards.Get([10, 20, 30], index));

        Assert.Equal("index", ex.ParamName);
        Assert.Equal(index, ex.ActualValue);
        Assert.Contains("Index must be below 3.", ex.Message);
    }

    [Fact]
    public async Task ShouldSaveAfterValidating() =>
        Assert.Equal("saved:/tmp/x", await Guards.SaveAsync("/tmp/x"));

    [Fact]
    public void ShouldThrowAtTheCallInsteadOfFaultingTheTask() =>
        Assert.Throws<ArgumentNullException>(() => { _ = Guards.SaveAsync(null!); });

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Boom() => throw new InvalidOperationException("boom");

    [Fact]
    public void ShouldLogAndRethrowKeepingTheOriginalStackTrace()
    {
        var log = new List<string>();

        var ex = Assert.Throws<InvalidOperationException>(() => Guards.LogAndRethrow(Boom, log));

        Assert.Equal(["boom"], log);
        Assert.Contains(nameof(Boom), ex.StackTrace);
    }

    [Fact]
    public void ShouldNotLogWhenNothingFails()
    {
        var log = new List<string>();

        Guards.LogAndRethrow(() => { }, log);

        Assert.Empty(log);
    }

    [Fact]
    public void ShouldLoadAValidSetting() =>
        Assert.Equal(42, Guards.LoadSetting("42"));

    [Fact]
    public void ShouldWrapAParseFailureKeepingTheOriginalAsInnerException()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Guards.LoadSetting("x"));

        Assert.Equal("Loading settings failed.", ex.Message);
        Assert.IsType<FormatException>(ex.InnerException);
    }
}
