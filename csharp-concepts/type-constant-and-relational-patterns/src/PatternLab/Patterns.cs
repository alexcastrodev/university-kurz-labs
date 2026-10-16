namespace PatternLab;

public enum Light { Red, Yellow, Green }

public static class Patterns
{
    public static string Describe(object? value)
    {
        // TODO-00: Use a switch expression with type, constant and relational patterns:
        //   null                -> "nothing"
        //   int below zero      -> "negative"
        //   int equal to zero   -> "zero"
        //   int above zero      -> "positive"
        //   "" (empty string)   -> "empty text"
        //   any other string    -> "text"
        //   bool                -> "yes" or "no"
        //   anything else       -> "other"
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Grade(int score)
    {
        // TODO-01: Use relational patterns in a switch expression:
        //   below 0 or above 100 -> throw ArgumentOutOfRangeException
        //   90 and above -> "A", 80 and above -> "B", 70 and above -> "C", otherwise "F"
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool IsWeekday(DayOfWeek day)
    {
        // TODO-02: Write it as a single `is not (... or ...)` pattern.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool IsAsciiLetter(char c)
    {
        // TODO-03: Write it as one `is` pattern combining ranges with `and` / `or`.
        //   Mind the parentheses: `and` binds tighter than `or`.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static decimal TicketPrice(int age, bool isMember)
    {
        // TODO-04: Switch on the tuple (age, isMember). Arms are tested top to bottom:
        //   age below 0 -> throw ArgumentOutOfRangeException
        //   below 3 -> 0, below 13 -> 5, 65 and above -> 7
        //   everyone else: members pay 8, non-members pay 10
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Next(Light light)
    {
        // TODO-05: Map Red -> "Green", Green -> "Yellow", Yellow -> "Red" with a switch expression
        //   that lists the three enum members and has NO discard arm. The compiler only warns
        //   when a member is missing, and an undefined value such as (Light)99 throws
        //   SwitchExpressionException at runtime.
        throw new NotImplementedException("Not implemented yet.");
    }
}
