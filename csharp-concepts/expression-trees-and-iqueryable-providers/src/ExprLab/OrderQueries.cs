namespace ExprLab;

public static class OrderQueries
{
    public static IQueryable<Order> Filter(IQueryable<Order> source, string? customer, decimal? minTotal)
    {
        // TODO-07: Add one Where per filter that has a value, and no Where at all for a null filter.
        // customer: exact match on Customer. minTotal: Total greater than or equal to it.
        throw new NotImplementedException("Not implemented yet.");
    }
}
