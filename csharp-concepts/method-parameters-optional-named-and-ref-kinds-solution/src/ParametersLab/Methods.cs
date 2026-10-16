namespace ParametersLab;

public static class Methods
{
    public static void Swap(ref int first, ref int second) =>
        (first, second) = (second, first);

    public static bool TryDivide(int dividend, int divisor, out int quotient, out int remainder)
    {
        if (divisor == 0)
        {
            quotient = 0;
            remainder = 0;
            return false;
        }

        (quotient, remainder) = Math.DivRem(dividend, divisor);
        return true;
    }

    public static ref readonly Particle Heaviest(Particle[] particles)
    {
        var heaviest = 0;
        for (var i = 1; i < particles.Length; i++)
        {
            if (particles[i].Mass > particles[heaviest].Mass)
            {
                heaviest = i;
            }
        }

        return ref particles[heaviest];
    }

    public static int Sum(params ReadOnlySpan<int> values)
    {
        var total = 0;
        foreach (var value in values)
        {
            total += value;
        }

        return total;
    }

    public static string BuildUrl(string host, int port = 80, bool secure = false) =>
        $"{(secure ? "https" : "http")}://{host}:{port}";

    public static string Pick(long value) => "long";

    public static string Pick(double value) => "double";

    public static string Pick(object value) => "object";

    // TODO-08 (optional)
    public static ref int FindSlot(int[] values, int target)
    {
        var index = Array.IndexOf(values, target);
        if (index < 0)
        {
            throw new KeyNotFoundException($"{target} is not in the array.");
        }

        return ref values[index];
    }

    // TODO-09 (optional)
    public static int CountValid(IEnumerable<string> texts) =>
        texts.Count(text => int.TryParse(text, out _));
}
