namespace OperatorFamiliesLab;

public static class Queries
{
    public static User? FindByEmail(IEnumerable<User> users, string email) =>
        users.SingleOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

    public static bool AllInStock(IEnumerable<Product> products)
    {
        var list = products.ToList();
        return list.Any() && list.All(p => p.Stock > 0);
    }

    public static Product? MostExpensive(IEnumerable<Product> products) =>
        products.MaxBy(p => p.Price);

    public static List<string> Ranking(IEnumerable<Player> players) =>
        players.OrderBy(p => p.Team, StringComparer.Ordinal)
            .ThenByDescending(p => p.Score)
            .ThenBy(p => p.Name, StringComparer.Ordinal)
            .Select(p => p.Name)
            .ToList();

    public static List<string> SortedKeys(IEnumerable<string> keys) =>
        keys.Order(StringComparer.Ordinal).ToList();

    public static List<T> Page<T>(IEnumerable<T> items, int pageNumber, int pageSize) =>
        items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

    public static List<int> BatchSums(IEnumerable<int> numbers, int batchSize) =>
        numbers.Chunk(batchSize).Select(batch => batch.Sum()).ToList();

    public static List<int> IntsOnly(IEnumerable<object?> items) =>
        items.OfType<int>().ToList();

    public static List<int> AllInts(IEnumerable<object?> items) =>
        items.Cast<int>().ToList();

    public static List<string> Describe(IEnumerable<string> words) =>
        (from w in words
         let len = w.Length
         where len > 3
         orderby len descending, w
         select $"{w}:{len}").ToList();

    public static List<int> RepeatedLengths(IEnumerable<string> words) =>
        (from w in words
         group w by w.Length into g
         where g.Count() > 1
         orderby g.Key
         select g.Key).ToList();
}
