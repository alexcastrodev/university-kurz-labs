namespace EventsLab;

public class Account
{
    private decimal _balance;

    public event EventHandler<ValueChangedEventArgs>? BalanceChanged;

    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        // TODO-00: Throw ArgumentOutOfRangeException when amount <= 0. Otherwise update the
        //   balance and call OnBalanceChanged with the old and the new balance.
        throw new NotImplementedException("Not implemented yet.");
    }

    protected virtual void OnBalanceChanged(ValueChangedEventArgs e)
    {
        // TODO-00: Raise BalanceChanged with `this` as sender, reading the event once with
        //   `?.Invoke`. With no subscribers nothing must happen.
        throw new NotImplementedException("Not implemented yet.");
    }
}
