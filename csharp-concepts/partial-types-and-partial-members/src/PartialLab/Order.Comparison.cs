namespace PartialLab;

// Second part of Order.
public partial class Order : IComparable<Order>
{
    public int CompareTo(Order? other)
    {
        // TODO-00: Make Order sealed from this part only (add `sealed` here, not in Order.cs),
        //   and order instances by Total. A null other sorts first (return 1).
        throw new NotImplementedException("Not implemented yet.");
    }
}
