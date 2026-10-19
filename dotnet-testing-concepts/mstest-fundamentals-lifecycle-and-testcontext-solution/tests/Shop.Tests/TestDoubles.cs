namespace Shop.Tests;

internal sealed class ApprovingGateway : IPaymentGateway
{
    public List<decimal> Charged { get; } = [];

    public Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken)
    {
        Charged.Add(amount);
        return Task.FromResult(true);
    }
}

internal sealed class DecliningGateway : IPaymentGateway
{
    public Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken) => Task.FromResult(false);
}

// Never answers: it only finishes when the token is cancelled, like a payment provider that hangs.
internal sealed class HangingGateway : IPaymentGateway
{
    public async Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout.Infinite, cancellationToken);
        return true;
    }
}
