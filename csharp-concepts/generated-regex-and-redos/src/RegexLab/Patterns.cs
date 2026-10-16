using System.Text.RegularExpressions;

namespace RegexLab;

public static partial class Patterns
{
    // TODO-00: Turn Date() into a generated regex (a `partial` method with [GeneratedRegex]).
    // It must capture the named groups y, m and d from text like 2025-11-11.
    public static Regex Date() => throw new NotImplementedException("Not implemented yet.");

    // TODO-01: This legacy pattern accepts only strings made of 'a', and it is vulnerable to
    // catastrophic backtracking. Make it a generated regex that keeps the 200 ms match timeout.
    public static Regex Legacy() => new(@"^(a+)+$", RegexOptions.None, TimeSpan.FromMilliseconds(200));

    // TODO-02: Rewrite the pattern so it accepts exactly the same strings as Legacy() but cannot
    // backtrack catastrophically. Generate it with a 1000 ms timeout (a safety net, not the fix).
    public static Regex Safe() => throw new NotImplementedException("Not implemented yet.");

    // TODO-03: Keep the Legacy pattern text, but run it on the non-backtracking engine.
    // Generate it, then check what type the generator actually gave you.
    public static Regex Linear() => throw new NotImplementedException("Not implemented yet.");
}
