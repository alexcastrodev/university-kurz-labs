using System.Linq.Expressions;

namespace ExprLab;

public static class Combinators
{
    public static Expression<Func<T, bool>> Or<T>(
        this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        // TODO-03: Rewrite right.Body so it uses left's parameter, then combine both bodies with
        // Expression.OrElse. Two lambdas own two different parameter objects, even when both are named "o".
        throw new NotImplementedException("Not implemented yet.");
    }
}

public sealed class ReplaceParameter(ParameterExpression from, ParameterExpression to) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node)
    {
        // TODO-02: Return `to` when the node is `from`, otherwise return the node unchanged.
        throw new NotImplementedException("Not implemented yet.");
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
        // TODO-04: Record node.Member.Name (once per name, in order of appearance), then keep walking.
        throw new NotImplementedException("Not implemented yet.");
    }
}
