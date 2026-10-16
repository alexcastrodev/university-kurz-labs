using System.Collections;
using System.Linq.Expressions;

namespace ExprLab;

public sealed class LoggingQueryable<T>(IQueryable<T> inner, List<string> log) : IOrderedQueryable<T>
{
    public Type ElementType => inner.ElementType;
    public Expression Expression => inner.Expression;
    public IQueryProvider Provider { get; } = new LoggingProvider(inner.Provider, log);

    public IEnumerator<T> GetEnumerator()
    {
        log.Add($"enumerate: {inner.Expression}");
        return inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class LoggingProvider(IQueryProvider inner, List<string> log) : IQueryProvider
{
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        // TODO-05: Log "CreateQuery: <name of the called method>" (the expression is a MethodCallExpression),
        // then delegate to inner and wrap the result so the next operator is logged too.
        throw new NotImplementedException("Not implemented yet.");
    }

    public TResult Execute<TResult>(Expression expression)
    {
        // TODO-06: Log "Execute: <expression>" and delegate to inner.
        throw new NotImplementedException("Not implemented yet.");
    }

    public IQueryable CreateQuery(Expression expression) => inner.CreateQuery(expression);
    public object? Execute(Expression expression) => inner.Execute(expression);
}
