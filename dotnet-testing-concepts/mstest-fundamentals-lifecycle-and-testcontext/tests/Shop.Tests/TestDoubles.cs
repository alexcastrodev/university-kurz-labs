namespace Shop.Tests;

// Hand-written fakes of the payment gateway: no mocking library, a few lines each.

// TODO-04: Approve every charge and remember each amount in a list the tests can read.
internal sealed class ApprovingGateway : IPaymentGateway
{
    public Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-04: approve and record the charge.");
}

// TODO-04: Decline every charge.
internal sealed class DecliningGateway : IPaymentGateway
{
    public Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-04: decline the charge.");
}

// TODO-04: Never answer, like a payment provider that hangs: only finish when the token is cancelled.
// Hint: await Task.Delay(Timeout.Infinite, cancellationToken).
internal sealed class HangingGateway : IPaymentGateway
{
    public Task<bool> ChargeAsync(decimal amount, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-04: wait until the token is cancelled.");
}
