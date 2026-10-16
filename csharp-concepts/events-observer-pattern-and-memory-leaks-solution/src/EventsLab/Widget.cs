using System.Runtime.CompilerServices;

namespace EventsLab;

public sealed class Widget : IDisposable
{
    private readonly TickSource _source;

    public int Ticks { get; private set; }

    public Widget(TickSource source)
    {
        _source = source;
        _source.Tick += OnTick;
    }

    public void OnTick(object? sender, EventArgs e) => Ticks++;

    public void Dispose() => _source.Tick -= OnTick;
}

public static class WidgetFactory
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static WeakReference Create(TickSource source, bool dispose)
    {
        var widget = new Widget(source);
        if (dispose) widget.Dispose();
        return new WeakReference(widget);
    }
}

public sealed class LambdaListener
{
    private EventHandler? _handler;

    public int Count { get; private set; }

    public void Attach(TickSource source)
    {
        _handler = (_, _) => Count++;
        source.Tick += _handler;
    }

    public void Detach(TickSource source) => source.Tick -= _handler;
}
