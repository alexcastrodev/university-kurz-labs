using System.Runtime.InteropServices;

namespace GenericsLab;

public static class Generics
{
    public static T Max<T>(IEnumerable<T> items)
    {
        // TODO-00: Return the largest item. Throw InvalidOperationException when the sequence
        //   is empty. Constrain T so that CompareTo is available without a cast (IComparable<T>).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T Create<T>()
    {
        // TODO-01: Return a new T. The compiler only allows `new T()` when T is constrained to
        //   a reference type with a public parameterless constructor (class, new()).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int ByteCount<T>(T value)
    {
        // TODO-02: Return the size in bytes of the value. Reinterpret it as a one-element
        //   ReadOnlySpan<T> (new ReadOnlySpan<T>(in value)) and use MemoryMarshal.AsBytes.
        //   That only compiles when T is unmanaged.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Describe<T>()
    {
        // TODO-03: Return "<type name>:<default value>", for example "Int32:0" and "String:null".
        //   Generics are reified, so typeof(T) and default(T) work at run time.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int CompareViaInterface(IComparable<int> value, int other) => value.CompareTo(other);

    public static int CompareViaConstraint<T>(T value, T other)
    {
        // TODO-05: Same comparison as CompareViaInterface, but through a constrained type
        //   parameter (IComparable<T>) so an int is never boxed.
        throw new NotImplementedException("Not implemented yet.");
    }
}

public static class TypeCounter<T>
{
    public static int Hits { get; private set; }

    public static void Increment()
    {
        // TODO-04: Count one hit for this closed type. Each TypeCounter<X> has its own static
        //   state, so TypeCounter<int> and TypeCounter<string> never share Hits.
        throw new NotImplementedException("Not implemented yet.");
    }
}
