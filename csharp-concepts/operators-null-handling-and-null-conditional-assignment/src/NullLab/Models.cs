namespace NullLab;

public class Customer
{
    public required string Name { get; set; }

    public string? Nickname { get; set; }

    public int Age { get; set; }
}

public class Order
{
    public Customer? Customer { get; set; }
}

public class Button
{
    public event Action? Clicked;

    public void Click() => Clicked?.Invoke();
}
