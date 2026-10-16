namespace GuardLab;

public class Customer
{
    public string? Email { get; set; }
}

public class Order
{
    public Customer? Customer { get; set; }

    public int Quantity { get; set; }

    public decimal Discount { get; set; }
}
