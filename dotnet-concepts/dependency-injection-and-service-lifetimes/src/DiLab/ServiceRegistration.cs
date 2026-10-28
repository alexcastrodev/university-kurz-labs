using Microsoft.Extensions.DependencyInjection;

namespace DiLab;

public static class ServiceRegistration
{
    public static IServiceCollection AddGameStoreServices(this IServiceCollection services)
    {
        // TODO-00: RequestState and FakeDb must live exactly as long as one request (one scope).
        services.AddScoped<RequestState>();
        services.AddScoped<FakeDb>();
        
        // TODO-01: ISystemClock is stateless and shared by everyone: one instance for the app.
        services.AddSingleton<ISystemClock, SystemClock>();
        
        // TODO-02: ReceiptNumberGenerator must hand out a new instance every time it is requested.
        services.AddTransient<ReceiptNumberGenerator>();
        
        // TODO-03: Register CatalogReader as a singleton, then run the tests. Read the
        //   exception about the scoped service and fix CatalogReader (see Services.cs).
        services.AddSingleton<CatalogReader>();
        
        // TODO-05 (optional): Register StripeGateway and PaypalGateway as keyed singletons
        //   ("stripe" and "paypal"), and CheckoutService as scoped.
        services.AddKeyedSingleton<IPaymentGateway, StripeGateway>("stripe");
        services.AddKeyedSingleton<IPaymentGateway, PaypalGateway>("paypal");
        services.AddScoped<CheckoutService>();
        
        return services;
    }
}
