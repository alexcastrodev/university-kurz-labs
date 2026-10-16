using System.Diagnostics.CodeAnalysis;

namespace FieldsLab;

public class Base
{
    public string SeenByBase { get; }

    public Base() => SeenByBase = Describe();

    protected virtual string Describe() => "Base";
}

public class Derived : Base
{
    private string _name = "initialized";
    private string? _late;

    public Derived() => _late = "set";

    protected override string Describe() => $"name={_name};late={_late ?? "null"}";

    public string Now() => Describe();
}

public class Limits
{
    public const int MaxRetries = 3;
    public static readonly int Timeout = 30;
}

public class Precise
{
    public static readonly int Value = 1;

    static Precise() { }
}

public class Relaxed
{
    public static readonly int Value = 1;
}

public class Person
{
    public required string Name { get; init; }
    public required int Born { get; init; }

    public Person() { }

    [SetsRequiredMembers]
    public Person(string name, int born)
    {
        Name = name;
        Born = born;
    }
}

public static class Counter<T>
{
    private static int _count;

    public static int Next() => ++_count;
}

public class Settings
{
    public string Host { get; }
    public int Port { get; }

    public Settings() : this("localhost", 5432) { }

    public Settings(string host) : this(host, 5432) { }

    public Settings(string host, int port)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(port);
        Host = host;
        Port = port;
    }
}
