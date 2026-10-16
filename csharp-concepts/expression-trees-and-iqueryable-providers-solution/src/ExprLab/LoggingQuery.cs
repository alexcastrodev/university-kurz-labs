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
        log.Add($"CreateQuery: {((MethodCallExpression)expression).Method.Name}");
        return new LoggingQueryable<TElement>(inner.CreateQuery<TElement>(expression), log);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        log.Add($"Execute: {expression}");
        return inner.Execute<TResult>(expression);
    }

    public IQueryable CreateQuery(Expression expression) => inner.CreateQuery(expression);
    public object? Execute(Expression expression) => inner.Execute(expression);
}
