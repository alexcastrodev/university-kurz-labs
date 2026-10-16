using System.Runtime.CompilerServices;

namespace EventsLab;

public sealed class Widget : IDisposable
{
    private readonly TickSource _source;

    public int Ticks { get; private set; }

    public Widget(TickSource source)
    {
        // TODO-01: Keep the source and subscribe OnTick to its Tick event.
        throw new NotImplementedException("Not implemented yet.");
    }

    public void OnTick(object? sender, EventArgs e) => Ticks++;

    public void Dispose()
    {
        // TODO-01: Unsubscribe OnTick from the source (the same method, so -= finds it).
        throw new NotImplementedException("Not implemented yet.");
    }
}

public static class WidgetFactory
{
    // TODO-03: Create a Widget on the source. When `dispose` is true, dispose it. Return a
    //   WeakReference to the widget and keep NO local reference to it after the method returns.
    //   Without NoInlining the JIT may keep the widget alive in the caller's frame.
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WeakReference Create(TickSource source, bool dispose)
    {
        throw new NotImplementedException("Not implemented yet.");
    }
}

public sealed class LambdaListener
{
    public int Count { get; private set; }

    public void Attach(TickSource source)
    {
        // TODO-04: Subscribe a handler that increments Count. Store the delegate (or use a
        //   method group) so that Detach can remove exactly the same instance.
        throw new NotImplementedException("Not implemented yet.");
    }

    public void Detach(TickSource source)
    {
        // TODO-04: Unsubscribe the handler added by Attach. Writing the same lambda again would
        //   create a different delegate and remove nothing.
        throw new NotImplementedException("Not implemented yet.");
    }
}
