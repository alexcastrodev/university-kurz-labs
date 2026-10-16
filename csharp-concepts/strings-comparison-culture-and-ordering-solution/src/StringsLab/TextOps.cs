using System.Globalization;

namespace StringsLab;

public static class TextOps
{
    public static string[] SortForPerson(IEnumerable<string> words, CultureInfo culture) =>
        [.. words.Order(StringComparer.Create(culture, CompareOptions.None))];

    public static string[] SortOrdinal(IEnumerable<string> words) =>
        [.. words.Order(StringComparer.Ordinal)];

    public static bool SameIdentifier(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    public static bool SameText(string a, string b) =>
        string.Equals(a, b, StringComparison.InvariantCulture);

    public static string LowerProtocol(string text) => text.ToLowerInvariant();

    public static string[] NaturalOrder(IEnumerable<string> names) =>
        [.. names.Order(StringComparer.Create(CultureInfo.InvariantCulture, CompareOptions.NumericOrdering))];

    public static bool ContainsIgnoringAccents(string text, string term) =>
        CultureInfo.InvariantCulture.CompareInfo.IndexOf(
            text, term, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) >= 0;

    public static string Canonical(string value) => string.Intern(value);
}
