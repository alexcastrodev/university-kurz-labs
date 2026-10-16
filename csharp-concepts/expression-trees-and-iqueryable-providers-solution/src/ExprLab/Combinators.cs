using System.Linq.Expressions;

namespace ExprLab;

public static class Combinators
{
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = left.Parameters[0];
        var rightBody = new ReplaceParameter(right.Parameters[0], parameter).Visit(right.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, rightBody), parameter);
    }

    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var parameter = left.Parameters[0];
        var rightBody = new ReplaceParameter(right.Parameters[0], parameter).Visit(right.Body);
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, rightBody), parameter);
    }
}

public sealed class ReplaceParameter(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == from ? to : node;
    }
}

public sealed class MemberCollector : ExpressionVisitor
{
    private readonly List<string> _names = [];

    public IReadOnlyList<string> Names => _names;

    public static IReadOnlyList<string> Collect(Expression expression)
    {
        var collector = new MemberCollector();
        collector.Visit(expression);
        return collector.Names;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (!_names.Contains(node.Member.Name)) _names.Add(node.Member.Name);
        return base.VisitMember(node);
    }
}
