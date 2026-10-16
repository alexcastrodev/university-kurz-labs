using System.Text.RegularExpressions;

namespace RegexLab;

public enum ValidationResult { Valid, Invalid, TooLong, TimedOut }

public static class InputValidator
{
    public const int MaxLength = 2048;

    public static ValidationResult Check(string input)
    {
        // TODO-04: Reject input longer than MaxLength before running any regex.

        // TODO-05: Match with Patterns.Legacy(). A RegexMatchTimeoutException must become
        // ValidationResult.TimedOut instead of escaping to the caller.

        throw new NotImplementedException("Not implemented yet.");
    }
}
