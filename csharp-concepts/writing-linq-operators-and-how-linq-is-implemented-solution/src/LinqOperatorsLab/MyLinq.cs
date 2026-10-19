namespace LinqOperatorsLab;

public static class MyLinq
{
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        return Iterate(source, predicate);

        static IEnumerable<T> Iterate(IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
                if (predicate(item)) yield return item;
        }
    }

    public static IEnumerable<TResult> MySelect<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        return Iterate(source, selector);

        static IEnumerable<TResult> Iterate(IEnumerable<T> source, Func<T, TResult> selector)
        {
            foreach (var item in source)
                yield return selector(item);
        }
    }

    public static IEnumerable<T> MyTake<T>(this IEnumerable<T> source, int count)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        return Iterate(source, count);

        static IEnumerable<T> Iterate(IEnumerable<T> source, int count)
        {
            if (count == 0) yield break;
            foreach (var item in source)
            {
                yield return item;
                if (--count == 0) yield break;   // leaving foreach disposes the source enumerator
            }
        }
    }

    public static IEnumerable<T> MyDistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        return Iterate(source, keySelector);

        static IEnumerable<T> Iterate(IEnumerable<T> source, Func<T, TKey> keySelector)
        {
            var seen = new HashSet<TKey>();   // created per enumeration, not in the wrapper
            foreach (var item in source)
                if (seen.Add(keySelector(item))) yield return item;
        }
    }

    public static IEnumerable<T[]> MyChunk<T>(this IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        return Iterate(source, size);

        static IEnumerable<T[]> Iterate(IEnumerable<T> source, int size)
        {
            var buffer = new List<T>(size);
            foreach (var item in source)
            {
                buffer.Add(item);
                if (buffer.Count == size)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }
            if (buffer.Count > 0) yield return buffer.ToArray();
        }
    }

    public static int MyCount<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (source is ICollection<T> collection) return collection.Count;

        var count = 0;
        foreach (var _ in source) count++;
        return count;
    }

    public static T MyElementAt<T>(this IEnumerable<T> source, int index)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        if (source is IList<T> list)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, list.Count);
            return list[index];
        }

        foreach (var item in source)
            if (index-- == 0) return item;
        throw new ArgumentOutOfRangeException(nameof(index));
    }
}
