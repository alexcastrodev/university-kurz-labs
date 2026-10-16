namespace ValueTypesLab;

public struct Counter
{
    public int Value;

    public void Increment() => Value++;
}

public readonly struct Temperature
{
    public double Celsius { get; init; }

    public Temperature WithCelsius(double celsius) => this with { Celsius = celsius };
}

public class Holder
{
    private Counter _counter;

    public int IncrementTwice()
    {
        _counter.Increment();
        _counter.Increment();
        return _counter.Value;
    }
}

public readonly struct Vector3D(double x, double y, double z)
{
    public double X { get; } = x;
    public double Y { get; } = y;
    public double Z { get; } = z;
}
