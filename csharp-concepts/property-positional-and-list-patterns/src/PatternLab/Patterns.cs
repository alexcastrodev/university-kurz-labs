namespace PatternLab;

public static class Patterns
{
    public static bool IsLisbonRegular(Customer customer)
    {
        // TODO-00: True when the customer lives in "Lisbon" and has more than zero orders. Use ONE
        //   property pattern with the extended form (Address.City). Address may be null: the
        //   pattern must answer false, not throw.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Quadrant(Point point)
    {
        // TODO-01: Positional switch on the Point (it is a record, so Deconstruct exists). Return
        //   "origin", "x-axis", "y-axis", "Q1" (x > 0, y > 0), "Q2" (x < 0, y > 0), "Q3" (x < 0,
        //   y < 0) and "Q4" for the rest. Use relational patterns and discards.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static State Next(State state, Trigger trigger)
    {
        // TODO-02: State machine as a tuple switch: Closed+Open -> Open, Open+Close -> Closed,
        //   Closed+Lock -> Locked, Locked+Unlock -> Closed. Anything else throws
        //   InvalidOperationException with the message "{trigger} is not valid in {state}".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Describe(int[] numbers)
    {
        // TODO-03: List patterns. [] -> "empty"; one element -> "one: 7"; otherwise
        //   "first 1, rest [2,3,4]" (capture the slice with `.. var rest`, join with ",").
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string? Inner(string text)
    {
        // TODO-04: Return what is between a leading '(' and a trailing ')', or null when the text is
        //   not wrapped. "(abc)" -> "abc", "()" -> "", "(abc" -> null, ")" -> null. One list pattern
        //   on the string with a slice.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Parse(string line)
    {
        // TODO-05: Split the line on spaces (ignore empty entries) and switch on the resulting array:
        //   ["move", x, y] with two valid ints -> "move by (3, 4)"; ["say", ..words] -> "say 'hello world'";
        //   ["quit"] or ["exit"] -> "bye"; any other first word -> "unknown command 'jump'"; no words -> "empty line".
        //   Order matters: a `move` with bad numbers must fall through to "unknown command 'move'".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool IsOrigin(object value)
    {
        // TODO-06: True for the boxed tuple (0, 0) and for a Point at the origin. `value` is typed as
        //   object: a positional pattern on a boxed ValueTuple works through ITuple, and a Point
        //   needs a type pattern first (Point(0, 0)).
        throw new NotImplementedException("Not implemented yet.");
    }
}
