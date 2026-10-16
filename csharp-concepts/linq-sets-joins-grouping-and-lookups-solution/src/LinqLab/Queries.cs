namespace LinqLab;

public static class Queries
{
    public static List<(string Customer, decimal Total)> InnerJoin(
        IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
        customers
            .Join(orders, c => (int?)c.Id, o => o.CustomerId, (c, o) => (c.Name, o.Total))
            .ToList();

    public static List<(string Customer, decimal? Total)> LeftJoin(
        IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
        customers
            .LeftJoin(orders, c => (int?)c.Id, o => o.CustomerId,
                (c, o) => (c.Name, o?.Total))
            .ToList();

    public static List<(string Customer, int OrderCount)> CountOrders(
        IEnumerable<Customer> customers, IEnumerable<Order> orders) =>
        customers
            .GroupJoin(orders, c => (int?)c.Id, o => o.CustomerId, (c, os) => (c.Name, os.Count()))
            .ToList();

    public static ILookup<int?, Order> OrdersByCustomer(IEnumerable<Order> orders) =>
        orders.ToLookup(o => o.CustomerId);

    public static List<Customer> NewCustomers(IEnumerable<Customer> known, IEnumerable<Customer> incoming) =>
        incoming
            .ExceptBy(known.Select(c => c.Email), c => c.Email, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public static List<(int CustomerId, decimal Total)> TotalPerCustomer(IEnumerable<Order> orders) =>
        orders
            .Where(o => o.CustomerId is not null)
            .GroupBy(o => o.CustomerId!.Value, o => o.Total)
            .Select(g => (g.Key, g.Sum()))
            .OrderBy(t => t.Key)
            .ToList();
}
