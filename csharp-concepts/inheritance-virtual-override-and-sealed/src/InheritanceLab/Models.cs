namespace InheritanceLab;

public class Animal
{
    // TODO-00: Make Speak() virtual so a subclass can replace it.
    public string Speak() => "Animal.Speak";

    // Describe() stays non-virtual on purpose: Dog hides it with `new`.
    public string Describe() => "Animal.Describe";

    // TODO-02: Make Clone() virtual.
    public Animal Clone() => new Animal();
}

public class Dog : Animal
{
    // TODO-00: Replace the hiding method with an override. It must return "Dog.Speak > " followed
    //   by the base implementation (use base.Speak()).
    public new string Speak() => "Dog.Speak";

    public new string Describe() => "Dog.Describe";

    // TODO-02: Override Clone() with a covariant return type: Dog Clone(), no cast for callers.
}

// TODO-01: Make Report abstract with a protected constructor and an abstract Title().
public class Report
{
    private readonly DateTime _created = new(2026, 10, 14);

    public Report() { }

    public string Render() => $"{Title()} @ {_created:yyyy-MM-dd}";

    protected virtual string Title() => "Untitled";
}

// TODO-01: SalesReport is a leaf: make it sealed.
public class SalesReport : Report
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

    // TODO-05 (optional): Stop further overriding: make this a `sealed override`.
    public override double Area() => Math.PI * Radius * Radius;
}

public class Square(double side) : Shape
{
    public double Side { get; } = side;

    public override double Area() => Side * Side;
}

public static class ShapeText
{
    // TODO-03: Use a switch expression with type patterns, in this order:
    //   Circle with Radius > 10 => "big circle", other Circle => $"circle {radius}",
    //   Square => $"square {side}", anything else (including null) => "unknown".
    public static string Describe(Shape? shape) => "unknown";
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

// TODO-04: Fragile base class. AddRange of three items reports Added == 6 because Bag.AddRange
//   calls the virtual Add again. Do not inherit from Bag: make CountingBag sealed, hold its own
//   List<string> (composition) and expose only Add, AddRange, Added and Count, counting each
//   item exactly once.
public class CountingBag : Bag
{
    public int Added { get; private set; }

    public override void Add(string item)
    {
        Added++;
        base.Add(item);
    }

    public override void AddRange(IEnumerable<string> items)
    {
        Added += items.Count();
        base.AddRange(items);
    }
}
