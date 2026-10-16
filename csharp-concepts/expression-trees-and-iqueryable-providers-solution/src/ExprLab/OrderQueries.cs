using System.Linq.Expressions;

namespace ExprLab;

public static class OrderQueries
{
    public static IQueryable<Order> Filter(IQueryable<Order> source, string? customer, decimal? minTotal)
    {
        var query = source;
        if (customer is not null) query = query.Where(o => o.Customer == customer);
        if (minTotal is not null) query = query.Where(o => o.Total >= minTotal);
        return query;
    }

    public static IQueryable<Order> OrderByProperty(IQueryable<Order> source, string property, bool descending = false)
    {
        var o = Expression.Parameter(typeof(Order), "o");
        var member = Expression.Property(o, property);
        var keySelector = Expression.Lambda(member, o);
        var operatorName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var method = typeof(Queryable).GetMethods()
            .Single(m => m.Name == operatorName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Order), member.Type);

        return source.Provider.CreateQuery<Order>(
            Expression.Call(null, method, source.Expression, Expression.Quote(keySelector)));
    }
}
