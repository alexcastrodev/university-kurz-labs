namespace EventsLab;

public class Account
{
    private decimal _balance;

    public event EventHandler<ValueChangedEventArgs>? BalanceChanged;

    public decimal Balance => _balance;

    public void Deposit(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(amount, 0m);
        var old = _balance;
        _balance += amount;
        OnBalanceChanged(new ValueChangedEventArgs(old, _balance));
    }

    protected virtual void OnBalanceChanged(ValueChangedEventArgs e) => BalanceChanged?.Invoke(this, e);
}
