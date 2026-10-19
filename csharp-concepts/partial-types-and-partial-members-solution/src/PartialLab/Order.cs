namespace PartialLab;

public abstract class Entity
{
    public int Id { get; init; }
}

// First part: base class and data. The second part lives in Order.Comparison.cs.
public partial class Order : Entity
{
    public decimal Total { get; init; }
}
