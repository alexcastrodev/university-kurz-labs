namespace PatternLab;

public static class Patterns
{
    public static bool IsLisbonRegular(Customer customer) =>
        customer is { Address.City: "Lisbon", Orders: > 0 };

    public static string Quadrant(Point point) => point switch
    {
        (0, 0) => "origin",
        (_, 0) => "x-axis",
        (0, _) => "y-axis",
        (> 0, > 0) => "Q1",
        (< 0, > 0) => "Q2",
        (< 0, < 0) => "Q3",
        _ => "Q4",
    };

    public static State Next(State state, Trigger trigger) => (state, trigger) switch
    {
        (State.Closed, Trigger.Open) => State.Open,
        (State.Open, Trigger.Close) => State.Closed,
        (State.Closed, Trigger.Lock) => State.Locked,
        (State.Locked, Trigger.Unlock) => State.Closed,
        _ => throw new InvalidOperationException($"{trigger} is not valid in {state}"),
    };

    public static string Describe(int[] numbers) => numbers switch
    {
        [] => "empty",
        [var only] => $"one: {only}",
        [var first, .. var rest] => $"first {first}, rest [{string.Join(",", rest)}]",
    };

    public static string? Inner(string text) =>
        text is ['(', .. var inner, ')'] ? inner : null;

    public static string Parse(string line) =>
        line.Split(' ', StringSplitOptions.RemoveEmptyEntries) switch
        {
            ["move", var x, var y] when int.TryParse(x, out var dx) && int.TryParse(y, out var dy)
                => $"move by ({dx}, {dy})",
            ["say", .. var words] => $"say '{string.Join(' ', words)}'",
            ["quit" or "exit"] => "bye",
            [var command, ..] => $"unknown command '{command}'",
            [] => "empty line",
        };

    public static bool IsOrigin(object value) =>
        value is (0, 0) or Point(0, 0);

    public static int Sum(ReadOnlySpan<int> numbers) => numbers switch
    {
        [] => 0,
        [var head, .. var tail] => head + Sum(tail),
    };

    public static bool IsGray(Rgb color) =>
        color is (var r, var g, var b) && r == g && g == b;
}
