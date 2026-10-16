using System.Linq.Expressions;

namespace ExprLab;

public static class Builders
{
    // TODO-00: Build `o => o.Total > value` by hand with Expression.Parameter, Expression.Property,
    // Expression.Constant, Expression.GreaterThan and Expression.Lambda. Name the parameter "o".
    public static Expression<Func<Order, bool>> TotalGreaterThan(decimal value) =>
        throw new NotImplementedException("Not implemented yet.");

    // Already working: the value is baked into the tree as a constant, so every different value
    // is a different tree (and, in EF Core, a different SQL text).
    public static Expression<Func<Order, bool>> TotalAboveConstant(decimal min)
    {
        var o = Expression.Parameter(typeof(Order), "o");
        return Expression.Lambda<Func<Order, bool>>(
            Expression.GreaterThan(Expression.Property(o, nameof(Order.Total)), Expression.Constant(min)), o);
    }

    // TODO-01: Same predicate as TotalAboveConstant, but the tree must reference the variable `min`
    // (a field on a closure object), not its current value. Hint: the body of `() => min` is exactly
    // that member access.
    public static Expression<Func<Order, bool>> TotalAboveCaptured(decimal min) =>
        throw new NotImplementedException("Not implemented yet.");
}
