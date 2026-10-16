using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace GuardLab;

public static class Guards
{
    public static string Place(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(order.Customer);
        ArgumentException.ThrowIfNullOrWhiteSpace(order.Customer.Email);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(order.Quantity);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(order.Discount, 0.5m);
        return $"placed {order.Quantity}";
    }

    public static T NotDefault<T>([NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? expression = null)
    {
        if (value is null || EqualityComparer<T>.Default.Equals(value, default!))
        {
            throw new ArgumentException($"'{expression}' must not be null or default.", expression);
        }

        return value;
    }

    public static string FirstArg(string[] args) =>
        args.Length > 0 ? args[0] : throw new ArgumentException("No arguments.");

    public static int Get(int[] values, int index)
    {
        if ((uint)index >= (uint)values.Length)
        {
            ThrowIndex(index, values.Length);
        }

        return values[index];
    }

    [DoesNotReturn]
    private static void ThrowIndex(int index, int length) =>
        throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be below {length}.");

    public static Task<string> SaveAsync(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return Core();

        async Task<string> Core()
        {
            await Task.Yield();
            return $"saved:{path}";
        }
    }

    public static void LogAndRethrow(Action action, List<string> log)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            log.Add(ex.Message);
            throw;
        }
    }

    public static int LoadSetting(string text)
    {
        try
        {
            return int.Parse(text);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Loading settings failed.", ex);
        }
    }

    public static void RethrowLater(Action action, List<ExceptionDispatchInfo> pending)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            pending.Add(ExceptionDispatchInfo.Capture(ex));
        }
    }
}
