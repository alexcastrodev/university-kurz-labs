using Microsoft.Extensions.DependencyInjection;

namespace DiLab.Tests;

// Optional TODOs live in their own class and use a provider without ValidateOnBuild,
// so a half-done optional TODO cannot break the required tests in LifetimeTests.
public class OptionalTodoTests
{
    private static ServiceProvider BuildProvider() =>
        new ServiceCollection()
            .AddGameStoreServices()
            .BuildServiceProvider();

    [Fact(Skip = "TODO-05 (optional): remove Skip once implemented")]
    public void ShouldResolveEachGatewayByItsKey()
    {
        using var provider = BuildProvider();

        Assert.IsType<StripeGateway>(provider.GetRequiredKeyedService<IPaymentGateway>("stripe"));
        Assert.IsType<PaypalGateway>(provider.GetRequiredKeyedService<IPaymentGateway>("paypal"));
    }

    [Fact(Skip = "TODO-05 (optional): remove Skip once implemented")]
    public void ShouldShareAKeyedSingletonAcrossScopes()
    {
        using var provider = BuildProvider();

        using var scopeA = provider.CreateScope();
        using var scopeB = provider.CreateScope();

        Assert.Same(
            scopeA.ServiceProvider.GetRequiredKeyedService<IPaymentGateway>("stripe"),
            scopeB.ServiceProvider.GetRequiredKeyedService<IPaymentGateway>("stripe"));
    }

    [Fact(Skip = "TODO-05 (optional): remove Skip once implemented")]
    public void ShouldInjectTheStripeGatewayIntoCheckoutService()
    {
        using var provider = BuildProvider();
        using var scope = provider.CreateScope();

        var checkout = scope.ServiceProvider.GetRequiredService<CheckoutService>();

        Assert.Equal("stripe", checkout.GatewayName);
    }
}
