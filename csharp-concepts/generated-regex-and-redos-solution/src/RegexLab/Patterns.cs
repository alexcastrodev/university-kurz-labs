using System.Text.RegularExpressions;

namespace RegexLab;

public static partial class Patterns
{
    [GeneratedRegex(@"(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})")]
    public static partial Regex Date();

    [GeneratedRegex(@"(?<y>\d{4})-(?<m>\d{2})-(?<d>\d{2})")]
    public static partial Regex DateProp { get; }

    [GeneratedRegex(@"^(a+)+$", RegexOptions.None, matchTimeoutMilliseconds: 200)]
    public static partial Regex Legacy();

    [GeneratedRegex(@"^a+$", RegexOptions.None, matchTimeoutMilliseconds: 1000)]
    public static partial Regex Safe();

    [GeneratedRegex(@"^(a+)+$", RegexOptions.NonBacktracking)]
    public static partial Regex Linear();

    public static int CountDates(string text) => Date().Count(text);
}
