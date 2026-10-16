namespace GcLab;

public static class Weak
{
    public static WeakReference<T> Of<T>(T value) where T : class
    {
        return new WeakReference<T>(value);
    }

    public static bool IsAlive<T>(WeakReference<T> reference) where T : class
    {
        return reference.TryGetTarget(out _);
    }
}

public sealed class Resurrector
{
    // The most recent instance saved by the finalizer.
    public static Resurrector? Saved;

    ~Resurrector()
    {
        Saved = this;
    }

    public static (WeakReference Short, WeakReference Long) Track(Resurrector instance)
    {
        return (new WeakReference(instance, trackResurrection: false),
                new WeakReference(instance, trackResurrection: true));
    }
}
