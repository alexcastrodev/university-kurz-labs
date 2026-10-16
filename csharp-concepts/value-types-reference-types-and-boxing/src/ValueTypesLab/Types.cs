namespace ValueTypesLab;

public struct Counter
{
    public int Value;

    public void Increment() => Value++;
}

public struct Temperature
{
    // TODO-02: Make Temperature a readonly struct. Celsius can then only be set at creation
    //   (init), and WithCelsius must return a NEW Temperature instead of changing this one.
    public double Celsius { get; set; }

    public Temperature WithCelsius(double celsius)
    {
        throw new NotImplementedException("Not implemented yet.");
    }
}

public class Holder
{
    // TODO-03: IncrementTwice returns 0 instead of 2. Calling a mutating method on a readonly
    //   struct field works on a hidden copy. Fix the declaration so the increments stick.
    private readonly Counter _counter;

    public int IncrementTwice()
    {
        _counter.Increment();
        _counter.Increment();
        return _counter.Value;
    }
}
