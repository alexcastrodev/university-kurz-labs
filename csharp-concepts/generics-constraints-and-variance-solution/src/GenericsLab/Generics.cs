using System.Runtime.InteropServices;

namespace GenericsLab;

public static class Generics
{
    public static T Max<T>(IEnumerable<T> items) where T : IComparable<T>
    {
        using var e = items.GetEnumerator();
        if (!e.MoveNext()) throw new InvalidOperationException("The sequence is empty.");

        var best = e.Current;
        while (e.MoveNext())
            if (e.Current.CompareTo(best) > 0) best = e.Current;
        return best;
    }

    public static T Create<T>() where T : class, new() => new();

    public static int ByteCount<T>(T value) where T : unmanaged =>
        MemoryMarshal.AsBytes(new ReadOnlySpan<T>(in value)).Length;

    public static string NameOf<T>(T value) where T : allows ref struct => typeof(T).Name;

    public static string Describe<T>() => $"{typeof(T).Name}:{default(T)?.ToString() ?? "null"}";

    public static int CompareViaInterface(IComparable<int> value, int other) => value.CompareTo(other);

    public static int CompareViaConstraint<T>(T value, T other) where T : IComparable<T> => value.CompareTo(other);
}

public static class TypeCounter<T>
{
    public static int Hits { get; private set; }

    public static void Increment() => Hits++;
}
