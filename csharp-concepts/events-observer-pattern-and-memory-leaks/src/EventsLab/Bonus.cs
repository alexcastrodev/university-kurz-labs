namespace EventsLab;

public sealed class Button
{
    private readonly List<EventHandler> _handlers = [];
    private readonly object _gate = new();

    public event EventHandler? Click
    {
        add
        {
            // TODO-05 (optional): Add the handler (ignore null) to _handlers under the lock.
            throw new NotImplementedException("Not implemented yet.");
        }
        remove
        {
            // TODO-05 (optional): Remove the handler (ignore null) from _handlers under the lock.
            throw new NotImplementedException("Not implemented yet.");
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
        // TODO-06 (optional): Subscribe only when `maybe` is not null, using the C# 14
        //   null-conditional assignment: maybe?.BalanceChanged += handler;
        throw new NotImplementedException("Not implemented yet.");
    }
}
