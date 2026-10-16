namespace GcLab;

public static class Weak
{
    public static WeakReference<T> Of<T>(T value) where T : class
    {
        // TODO-03: Return a WeakReference<T> to the value (it must not keep it alive).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool IsAlive<T>(WeakReference<T> reference) where T : class
    {
        // TODO-03: Return true while the target has not been collected (TryGetTarget).
        throw new NotImplementedException("Not implemented yet.");
    }
}

public sealed class Resurrector
{
    // The most recent instance saved by the finalizer.
    public static Resurrector? Saved;

    ~Resurrector()
    {
        // TODO-04: Resurrect the object by storing `this` in Saved. Do not throw here: an
        //   exception in a finalizer crashes the whole process.
    }

    public static (WeakReference Short, WeakReference Long) Track(Resurrector instance)
    {
        // TODO-04: Return two weak references to the instance: Short (does not track
        //   resurrection) and Long (trackResurrection: true).
        throw new NotImplementedException("Not implemented yet.");
    }
}
