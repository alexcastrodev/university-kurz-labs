namespace EventsLab;

public sealed class ValueChangedEventArgs(decimal oldValue, decimal newValue) : EventArgs
{
    public decimal OldValue { get; } = oldValue;
    public decimal NewValue { get; } = newValue;
}

// A publisher that is meant to live forever (think of a static application clock).
public sealed class TickSource
{
    public event EventHandler? Tick;

    public int HandlerCount => Tick?.GetInvocationList().Length ?? 0;

    public void Raise() => Tick?.Invoke(this, EventArgs.Empty);
}

public sealed class PriceFeed
{
    private event EventHandler<int>? PriceChanged;

    public int HandlerCount => PriceChanged?.GetInvocationList().Length ?? 0;

    public void Publish(int price) => PriceChanged?.Invoke(this, price);

    public IDisposable Subscribe(EventHandler<int> handler)
    {
        PriceChanged += handler;
        return new Subscription(() => PriceChanged -= handler);
    }

    private sealed class Subscription(Action unsubscribe) : IDisposable
    {
        public void Dispose() => unsubscribe();
    }
}
