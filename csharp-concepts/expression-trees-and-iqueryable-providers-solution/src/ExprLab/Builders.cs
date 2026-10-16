using System.Linq.Expressions;

namespace ExprLab;

public static class Builders
{
    public static Expression<Func<Order, bool>> TotalGreaterThan(decimal value)
    {
        var o = Expression.Parameter(typeof(Order), "o");
        return Expression.Lambda<Func<Order, bool>>(
            Expression.GreaterThan(Expression.Property(o, nameof(Order.Total)), Expression.Constant(value)), o);
    }

    // The value is baked into the tree as a constant, so every different value
    // is a different tree (and, in EF Core, a different SQL text).
    public static Expression<Func<Order, bool>> TotalAboveConstant(decimal min)
    {
        var o = Expression.Parameter(typeof(Order), "o");
        return Expression.Lambda<Func<Order, bool>>(
            Expression.GreaterThan(Expression.Property(o, nameof(Order.Total)), Expression.Constant(min)), o);
    }

    public static Expression<Func<Order, bool>> TotalAboveCaptured(decimal min)
    {
        Expression<Func<decimal>> holder = () => min;
        var o = Expression.Parameter(typeof(Order), "o");
        return Expression.Lambda<Func<Order, bool>>(
            Expression.GreaterThan(Expression.Property(o, nameof(Order.Total)), holder.Body), o);
    }
}
