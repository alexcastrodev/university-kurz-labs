namespace PatternLab;

public enum Light { Red, Yellow, Green }

public static class Patterns
{
    public static string Describe(object? value) => value switch
    {
        null => "nothing",
        int and < 0 => "negative",
        0 => "zero",
        int => "positive",
        "" => "empty text",
        string => "text",
        bool b => b ? "yes" : "no",
        _ => "other",
    };

    public static string Grade(int score) => score switch
    {
        < 0 or > 100 => throw new ArgumentOutOfRangeException(nameof(score)),
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        _ => "F",
    };

    public static bool IsWeekday(DayOfWeek day) =>
        day is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    public static bool IsAsciiLetter(char c) =>
        c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');

    public static decimal TicketPrice(int age, bool isMember) => (age, isMember) switch
    {
        (< 0, _) => throw new ArgumentOutOfRangeException(nameof(age)),
        (< 3, _) => 0m,
        (< 13, _) => 5m,
        (>= 65, _) => 7m,
        (_, true) => 8m,
        _ => 10m,
    };

    // CS8524 is expected: no discard arm, so (Light)99 throws SwitchExpressionException.
#pragma warning disable CS8524
    public static string Next(Light light) => light switch
    {
        Light.Red => "Green",
        Light.Green => "Yellow",
        Light.Yellow => "Red",
    };
#pragma warning restore CS8524
}
