namespace InheritanceLab;

public class Animal
{
    public virtual string Speak() => "Animal.Speak";

    public string Describe() => "Animal.Describe";

    public virtual Animal Clone() => new Animal();
}

public class Dog : Animal
{
    public override string Speak() => "Dog.Speak > " + base.Speak();

    public new string Describe() => "Dog.Describe";

    public override Dog Clone() => new Dog();
}

public abstract class Report
{
    private readonly DateTime _created = new(2026, 10, 14);

    protected Report() { }

    public string Render() => $"{Title()} @ {_created:yyyy-MM-dd}";

    protected abstract string Title();
}

public sealed class SalesReport : Report
{
    protected override string Title() => "Sales";
}

public abstract class Shape
{
    public abstract double Area();
}

public class Circle(double radius) : Shape
{
    public double Radius { get; } = radius;

    public sealed override double Area() => Math.PI * Radius * Radius;
}

public class Square(double side) : Shape
{
    public double Side { get; } = side;

    public override double Area() => Side * Side;
}

public static class ShapeText
{
    public static string Describe(Shape? shape) => shape switch
    {
        Circle { Radius: > 10 } => "big circle",
        Circle c => $"circle {c.Radius}",
        Square s => $"square {s.Side}",
        _ => "unknown",
    };
}

public class Bag
{
    private readonly List<string> _items = [];

    public int Count => _items.Count;

    public virtual void Add(string item) => _items.Add(item);

    public virtual void AddRange(IEnumerable<string> items)
    {
        foreach (var item in items) Add(item);
    }
}

public sealed class CountingBag
{
    private readonly List<string> _items = [];

    public int Added { get; private set; }

    public int Count => _items.Count;

    public void Add(string item)
    {
        Added++;
        _items.Add(item);
    }

    public void AddRange(IEnumerable<string> items)
    {
        foreach (var item in items) Add(item);
    }
}
