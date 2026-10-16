namespace EventsLab;

public sealed class Button
{
    private readonly List<EventHandler> _handlers = [];
    private readonly object _gate = new();

    public event EventHandler? Click
    {
        add
        {
            if (value is null) return;
            lock (_gate) _handlers.Add(value);
        }
        remove
        {
            if (value is null) return;
            lock (_gate) _handlers.Remove(value);
        }
    }

    public int HandlerCount
    {
        get { lock (_gate) return _handlers.Count; }
    }

    public void Press()
    {
        EventHandler[] snapshot;
        lock (_gate) snapshot = [.. _handlers];
        foreach (var handler in snapshot) handler(this, EventArgs.Empty);
    }
}

public static class Subscriber
{
    public static void Attach(Account? maybe, EventHandler<ValueChangedEventArgs> handler)
    {
        maybe?.BalanceChanged += handler;
    }
}
