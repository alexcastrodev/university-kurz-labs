namespace Ordering;

public sealed class Order
{
    private Order()
    {
    }

    public Order(string tenantId, string number, string customerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        ArgumentException.ThrowIfNullOrWhiteSpace(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
        TenantId = tenantId;
        Number = number;
        CustomerName = customerName;
    }

    public int Id { get; private set; }
    public string TenantId { get; private set; } = null!;
    public string Number { get; private set; } = null!;
    public string CustomerName { get; private set; } = null!;
    public bool IsDeleted { get; private set; }
    public List<OrderLine> Lines { get; private set; } = [];

    // Mapped to PostgreSQL's xmin system column: it changes on every update of the row.
    public uint Version { get; private set; }

    public void AddLine(string sku, int quantity, decimal unitPrice) =>
        Lines.Add(new OrderLine(sku, quantity, unitPrice));

    public void Delete() => IsDeleted = true;

    public void ChangeCustomer(string customerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
        CustomerName = customerName;
    }
}

public sealed class OrderLine
{
    private OrderLine()
    {
    }

    public OrderLine(string sku, int quantity, decimal unitPrice)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegative(unitPrice);
        Sku = sku;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string Sku { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
