namespace Orders.Api.Orders;

// Registered as a singleton: one list for the whole life of the application. It stands in for a database.
public sealed class OrderStore
{
    private readonly Lock _lock = new();
    private readonly List<Order> _orders = [];
    private int _lastId;

    public Order Add(string sku, int quantity)
    {
        lock (_lock)
        {
            var order = new Order(++_lastId, sku, quantity);
            _orders.Add(order);
            return order;
        }
    }

    public Order? Find(int id)
    {
        lock (_lock)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }
    }

    public Order? Latest()
    {
        lock (_lock)
        {
            return _orders.LastOrDefault();
        }
    }

    public IReadOnlyList<Order> All()
    {
        lock (_lock)
        {
            return [.. _orders];
        }
    }

    // What a test would do with a real database: truncate the tables before each test.
    public void Clear()
    {
        lock (_lock)
        {
            _orders.Clear();
            _lastId = 0;
        }
    }
}
