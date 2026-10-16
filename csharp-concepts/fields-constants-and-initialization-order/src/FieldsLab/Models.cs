using System.Diagnostics.CodeAnalysis;

namespace FieldsLab;

public class Base
{
    public string SeenByBase { get; }

    // TODO-00: Call the virtual Describe() from this constructor and store the result in SeenByBase.
    //   Then give Derived the override below and watch which of its fields is already set.
    public Base() => SeenByBase = "";

    protected virtual string Describe() => "Base";
}

public class Derived : Base
{
    // The field initializer runs BEFORE the base constructor; _late is only assigned in the
    // constructor body, which runs AFTER it.
    private string _name = "initialized";
    private string? _late;

    public Derived() => _late = "set";

    // TODO-00: Override Describe() to return $"name={_name};late={_late ?? "null"}".
    public string Now() => Describe();
}

public class Limits
{
    // TODO-01: MaxRetries can never change: make it a compile-time constant.
    public static int MaxRetries = 3;

    // TODO-01: Timeout is a tunable default that consumers must read at run time: make it
    //   static readonly (not const, a const would be copied into every compiled caller).
    public static int Timeout = 30;
}

public class Precise
{
    public static readonly int Value = 1;

    // TODO-02: Add an empty static constructor so the type is NOT marked beforefieldinit.
}

public class Relaxed
{
    // No static constructor on purpose: the compiler marks this type beforefieldinit.
    public static readonly int Value = 1;
}

public class Person
{
    // TODO-03: Make Name and Born `required` so object initializers must set them.
    public string Name { get; init; } = "";
    public int Born { get; init; }

    public Person() { }

    // TODO-03: This constructor assigns every required member: tell the compiler with
    //   [SetsRequiredMembers].
    public Person(string name, int born)
    {
        Name = name;
        Born = born;
    }
}

public static class Counter<T>
{
    // TODO-04 (optional): Every closed generic type (Counter<int>, Counter<string>) has its own
    //   statics. Store the count in a static field of Counter<T> itself so Counter<int>.Next(),
    //   Counter<int>.Next(), Counter<string>.Next() return 1, 2, 1.
    public static int Next() => ++Shared.Count;
}

public static class Shared
{
    public static int Count;
}

public class Settings
{
    public string Host { get; }
    public int Port { get; }

    // TODO-05 (optional): Chain the constructors: () uses "localhost" and 5432, (host) uses 5432,
    //   and only the full (host, port) constructor assigns the properties. Validate there:
    //   a blank host throws ArgumentException, a port <= 0 throws ArgumentOutOfRangeException.
    public Settings()
    {
        Host = "";
    }

    public Settings(string host)
    {
        Host = host;
    }

    public Settings(string host, int port)
    {
        Host = host;
        Port = port;
    }
}
