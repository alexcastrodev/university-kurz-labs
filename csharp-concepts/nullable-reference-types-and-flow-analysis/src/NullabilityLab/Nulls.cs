namespace NullabilityLab;

public static class Nulls
{
    private static readonly Dictionary<string, User> Users = new()
    {
        ["1"] = new User { Email = "ann@example.com", Nickname = "annie" },
        ["2"] = new User { Email = "bob@example.com" },
    };

    public static string DisplayName(User user)
    {
        // TODO-01: Return the nickname, or the email when the nickname is null or white space.
        //   Use string.IsNullOrWhiteSpace and no null-forgiving operator (!).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static User FindUser(string id)
    {
        // TODO-02: Return the user with this id, or null when it does not exist (the Users
        //   dictionary has GetValueOrDefault). Change the return type so it tells the truth.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool TryGetUser(string id, out User? user)
    {
        // TODO-03: Set user from FindUser and return whether it was found. Annotate the out
        //   parameter with [NotNullWhen(true)] so callers can use user after an if without a check.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string? Trim(string? value)
    {
        // TODO-05: Return the trimmed value, or null when value is null. Then annotate the return
        //   value with [NotNullIfNotNull(nameof(value))]: a non-null argument gives a non-null result.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Require(string? value)
    {
        // TODO-06: Throw ArgumentNullException when value is null, otherwise return it.
        //   The compiler already warns callers who pass null, but nothing is enforced at run
        //   time, so the guard clause is still needed.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Compare(int? a, int? b)
    {
        // TODO-07: Return "equal", "less" or "greater". When exactly one side is null the
        //   answer is "unordered", and two nulls are "equal". Lifted operators on int? already
        //   behave this way: check what a < b and a > b give when one side is null.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T? FirstOrNull<T>(IEnumerable<T> items)
    {
        // TODO-08: Return the first item, or null for an empty sequence. For T? to mean
        //   Nullable<T> the method needs a constraint on T. Add it.
        throw new NotImplementedException("Not implemented yet.");
    }
}
