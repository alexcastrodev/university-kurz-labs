namespace ClosuresLab;

public delegate bool TryParser<T>(string text, out T result);

public static class Functions
{
    public static Func<int> MakeCounter()
    {
        var count = 0;
        return () => ++count;
    }

    public static List<Func<int>> MakeReaders(int count)
    {
        var readers = new List<Func<int>>();
        for (var i = 0; i < count; i++)
        {
            var copy = i;
            readers.Add(() => copy);
        }

        return readers;
    }

    public static IEnumerable<int> Squares(IEnumerable<int> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return Iterate(source);

        static IEnumerable<int> Iterate(IEnumerable<int> items)
        {
            foreach (var item in items)
            {
                yield return item * item;
            }
        }
    }

    public static Func<string, int> IntParser() => int.Parse;

    public static Func<int, int> Memoize(Func<int, int> compute)
    {
        var cache = new Dictionary<int, int>();
        return argument =>
        {
            if (!cache.TryGetValue(argument, out var result))
            {
                result = compute(argument);
                cache[argument] = result;
            }

            return result;
        };
    }

    // TODO-05 (optional)
    public static string Greet(string name, bool formal)
    {
        var greet = (string person, string greeting = "Hello") => $"{greeting}, {person}";
        return formal ? greet(name, "Good evening") : greet(name);
    }

    // TODO-06 (optional)
    public static int ParseOrDefault(string text, int fallback)
    {
        TryParser<int> parse = (value, out result) => int.TryParse(value, out result);
        return parse(text, out var number) ? number : fallback;
    }

    // TODO-07 (optional)
    public static Func<int, int> Increment() => static value => value + 1;
}
