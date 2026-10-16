using System.Diagnostics.CodeAnalysis;

namespace NullabilityLab;

public static class Nulls
{
    private static readonly Dictionary<string, User> Users = new()
    {
        ["1"] = new User { Email = "ann@example.com", Nickname = "annie" },
        ["2"] = new User { Email = "bob@example.com" },
    };

    public static string DisplayName(User user) =>
        string.IsNullOrWhiteSpace(user.Nickname) ? user.Email : user.Nickname;

    public static User? FindUser(string id) => Users.GetValueOrDefault(id);

    public static bool TryGetUser(string id, [NotNullWhen(true)] out User? user)
    {
        user = FindUser(id);
        return user is not null;
    }

    [return: NotNullIfNotNull(nameof(value))]
    public static string? Trim(string? value) => value?.Trim();

    public static string Require(string? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value;
    }

    public static string Compare(int? a, int? b)
    {
        if (a == b) return "equal";
        if (a < b) return "less";
        if (a > b) return "greater";
        return "unordered";
    }

    public static T? FirstOrNull<T>(IEnumerable<T> items) where T : struct
    {
        foreach (var item in items) return item;
        return null;
    }

    public static bool TryFirst<T>(IEnumerable<T> items, Func<T, bool> predicate, [MaybeNullWhen(false)] out T value)
    {
        foreach (var item in items)
        {
            if (!predicate(item)) continue;
            value = item;
            return true;
        }

        value = default;
        return false;
    }

    public static Dictionary<TKey, int> CountBy<TKey>(IEnumerable<TKey> items) where TKey : notnull
    {
        var counts = new Dictionary<TKey, int>();
        foreach (var item in items) counts[item] = counts.GetValueOrDefault(item) + 1;
        return counts;
    }
}
