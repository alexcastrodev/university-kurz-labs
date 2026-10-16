namespace PartialLab;

// Second part: `sealed` and the interface are merged into the type declared in Order.cs.
public sealed partial class Order : IComparable<Order>
{
    public int CompareTo(Order? other) => other is null ? 1 : Total.CompareTo(other.Total);
}
