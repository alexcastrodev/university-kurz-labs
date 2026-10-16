using System.Diagnostics.CodeAnalysis;

namespace PropertiesLab;

public class Temperature
{
    private const double AbsoluteZero = -273.15;

    public double Celsius
    {
        get;
        set => field = value >= AbsoluteZero
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Below absolute zero.");
    }
}

public class Person
{
    public string Name
    {
        get;
        init
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            field = value.Trim();
        }
    } = "";
}

public class Document
{
    public string Title { get => field ??= "untitled"; set; } = null!;
}

public class Counter
{
    public event Action<int>? Changed;

    public int Value
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnChanged(value);
        }
    }

    protected void OnChanged(int value) => Changed?.Invoke(value);
}

public class Price
{
    // TODO-04 (optional)
    public decimal Amount { get; init => field = Math.Round(value, 2, MidpointRounding.AwayFromZero); }
}

public class Session
{
    // TODO-05 (optional)
    [field: NonSerialized]
    public string Token { get; set; } = "";
}
