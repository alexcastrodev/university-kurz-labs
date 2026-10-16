namespace ClosuresLab;

public static class Functions
{
    public static Func<int> MakeCounter()
    {
        // TODO-00: Return a function that returns 1, 2, 3... on successive calls. Keep the count
        //   in a local variable captured by the lambda. Each MakeCounter() call starts its own count.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static List<Func<int>> MakeReaders(int count)
    {
        // TODO-01: Build `count` functions in a `for` loop; function i must return i. The loop
        //   variable is shared by every iteration, so copy it into a local declared inside the
        //   body before capturing it.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IEnumerable<int> Squares(IEnumerable<int> source)
    {
        // TODO-02: Throw ArgumentNullException right away when source is null, even if nobody
        //   enumerates the result yet. An iterator (`yield return`) defers its body, so validate
        //   here and delegate the iteration to a `static` local function.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Func<string, int> IntParser()
    {
        // TODO-03: Return a Func<string, int> that parses text with int.Parse. Use the method
        //   group directly instead of wrapping it in a lambda.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Func<int, int> Memoize(Func<int, int> compute)
    {
        // TODO-04: Return a function that calls `compute` once per distinct argument and caches
        //   the result in a Dictionary captured by the returned lambda.
        throw new NotImplementedException("Not implemented yet.");
    }
}
