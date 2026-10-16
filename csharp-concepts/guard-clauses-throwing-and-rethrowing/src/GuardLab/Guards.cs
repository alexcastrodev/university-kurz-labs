using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace GuardLab;

public static class Guards
{
    public static string Place(Order order)
    {
        // TODO-00: Validate with the BCL ThrowIf helpers, one line each, in this order:
        //   order not null, order.Customer not null, order.Customer.Email not null/empty/whitespace,
        //   order.Quantity not negative or zero, order.Discount not greater than 0.5m.
        //   The ParamName of each exception must be the expression text ("order.Customer", ...).
        //   Then return $"placed {order.Quantity}".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static T NotDefault<T>([NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? expression = null)
    {
        // TODO-01: Throw ArgumentException when value is null or equals default(T)
        //   (use EqualityComparer<T>.Default). The message names the argument, and ParamName must be
        //   the caller's source text, which the compiler already gives you in `expression`.
        //   Otherwise return the value.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string FirstArg(string[] args)
    {
        // TODO-02: Return the first argument, or throw ArgumentException("No arguments.") when there
        //   is none. Do it as ONE expression: a conditional with a throw expression.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Get(int[] values, int index)
    {
        // TODO-03: Throw through ThrowIndex (below) when index is out of range, then return the
        //   element. Use the unsigned trick `(uint)index >= (uint)values.Length` to test both bounds.
        throw new NotImplementedException("Not implemented yet.");
    }

    [DoesNotReturn]
    private static void ThrowIndex(int index, int length)
    {
        // TODO-04: Throw ArgumentOutOfRangeException(nameof(index), index, $"Index must be below {length}.").
        //   Keeping the throw out of Get makes Get small enough to inline.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Task<string> SaveAsync(string path)
    {
        // TODO-05: Validate the path so that the exception is thrown AT THE CALL, not stored in the
        //   returned Task. Make this method non-async, validate, and return the task of an async local
        //   function that awaits Task.Yield() and returns $"saved:{path}".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void LogAndRethrow(Action action, List<string> log)
    {
        // TODO-06: Run the action. If it throws, add the exception message to the log and let the same
        //   exception continue WITHOUT resetting its stack trace (`throw;`, not `throw ex;`).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int LoadSetting(string text)
    {
        // TODO-07: Parse the text as an int. When it is not a valid number, throw
        //   InvalidOperationException("Loading settings failed.") that carries the FormatException
        //   as its inner exception, so the original stack trace is not lost.
        throw new NotImplementedException("Not implemented yet.");
    }
}
