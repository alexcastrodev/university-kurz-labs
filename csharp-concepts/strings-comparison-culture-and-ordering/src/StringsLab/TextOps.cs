using System.Globalization;

namespace StringsLab;

public static class TextOps
{
    public static string[] SortForPerson(IEnumerable<string> words, CultureInfo culture)
    {
        // TODO-00: Sort the way a person of that culture expects: build a cultural comparer with
        //   StringComparer.Create(culture, CompareOptions.None) and sort with it.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string[] SortOrdinal(IEnumerable<string> words)
    {
        // TODO-01: Sort by UTF-16 code unit, the same on every machine.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool SameIdentifier(string a, string b)
    {
        // TODO-02: Compare identifiers (header names, extensions) ignoring case but with no
        //   culture lookup at all.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool SameText(string a, string b)
    {
        // TODO-03: Compare text meant for people: canonically equivalent sequences (e + combining
        //   accent versus the precomposed letter) and zero-weight characters must count as equal.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string LowerProtocol(string text)
    {
        // TODO-04: Lower-case protocol text so the result never depends on the user's locale
        //   (the Turkish-i bug: "TITLE" must become "title" even under tr-TR).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string[] NaturalOrder(IEnumerable<string> names)
    {
        // TODO-05: Sort so that "file2" comes before "file10" and "Windows 8" before "Windows 10".
        //   Use the .NET 10 CompareOptions.NumericOrdering with the invariant culture.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool ContainsIgnoringAccents(string text, string term)
    {
        // TODO-06: Return true when term occurs in text ignoring case and diacritics. Use the
        //   invariant CompareInfo.IndexOf with CompareOptions.IgnoreNonSpace | IgnoreCase.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Canonical(string value)
    {
        // TODO-07: Return the pooled instance, so equal strings share one object.
        throw new NotImplementedException("Not implemented yet.");
    }
}
