using System.Text.RegularExpressions;

namespace RegexLab;

public enum ValidationResult { Valid, Invalid, TooLong, TimedOut }

public static class InputValidator
{
    public const int MaxLength = 2048;

    public static ValidationResult Check(string input)
    {
        if (input.Length > MaxLength) return ValidationResult.TooLong;

        try
        {
            return Patterns.Legacy().IsMatch(input) ? ValidationResult.Valid : ValidationResult.Invalid;
        }
        catch (RegexMatchTimeoutException)
        {
            return ValidationResult.TimedOut;
        }
    }
}
