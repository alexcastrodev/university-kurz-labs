namespace LinqOperatorsLab;

public static class MyLinq
{
    public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        // TODO-00: Validate source and predicate NOW (ArgumentNullException at the call, not at
        //   the first MoveNext), then return a private iterator method that does the `yield return`.
        //   A method containing `yield` cannot validate eagerly, so split it in two.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IEnumerable<TResult> MySelect<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
    {
        // TODO-00: Same split as MyWhere: eager validation in this method, lazy projection in a
        //   private iterator. The selector must run once per element, only when it is pulled.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IEnumerable<T> MyTake<T>(this IEnumerable<T> source, int count)
    {
        // TODO-01: Validate eagerly (null source, negative count). Then yield at most `count`
        //   items. Stop right after the last one: do NOT call MoveNext on the source once you
        //   have enough, and do not touch the source at all when count is 0. Leaving the
        //   iterator early must dispose the source enumerator (foreach/finally does it for you).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IEnumerable<T> MyDistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        // TODO-02: Yield each element whose key has not been seen yet, streaming (no buffering of
        //   the whole source). Run keySelector once per element. Keep the "seen" set INSIDE the
        //   iterator, so every enumeration starts empty. Validate arguments eagerly.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IEnumerable<T[]> MyChunk<T>(this IEnumerable<T> source, int size)
    {
        // TODO-03: Yield arrays of `size` items; the last one may be shorter. Pull from the source
        //   only as far as the consumer asks (the first chunk must pull exactly `size` items).
        //   Throw ArgumentOutOfRangeException eagerly when size < 1. The source enumerator must be
        //   disposed when the consumer stops early.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int MyCount<T>(this IEnumerable<T> source)
    {
        // TODO-04: If the source is an ICollection<T>, return its Count without creating an
        //   enumerator. Otherwise enumerate and count.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T MyElementAt<T>(this IEnumerable<T> source, int index)
    {
        // TODO-04: If the source is an IList<T>, use the indexer (no enumerator). Otherwise enumerate
        //   until the index. A negative or too large index throws ArgumentOutOfRangeException in
        //   both paths. The enumerator must be disposed.
        throw new NotImplementedException("Not implemented yet.");
    }
}
