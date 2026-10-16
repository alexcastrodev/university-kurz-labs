namespace PropertiesLab;

public class Temperature
{
    // TODO-00: Reject anything below absolute zero (-273.15) with ArgumentOutOfRangeException.
    //   Write the setter body with the `field` keyword (C# 14): no manual backing field needed.
    public double Celsius { get; set; }
}

public class Person
{
    // TODO-01: Make Name an init-only property that stores the trimmed value and throws
    //   ArgumentException for null, empty or whitespace text. Use `field` in the init accessor.
    public string Name { get; init; } = "";
}

public class Document
{
    // TODO-02: Return "untitled" until a title is assigned: lazy default in the getter with
    //   `field ??= "untitled"`. Keep the setter an auto-implemented `set;`.
    public string Title { get; set; } = null!;
}

public class Counter
{
    public event Action<int>? Changed;

    // TODO-03: Raise Changed with the new value, but only when the value actually changes.
    //   Assigning the current value again must not raise the event. Use `field` to read the
    //   old value inside the setter.
    public int Value { get; set; }

    protected void OnChanged(int value) => Changed?.Invoke(value);
}
