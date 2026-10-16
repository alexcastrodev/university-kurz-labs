namespace ParametersLab;

public static class Methods
{
    public static void Swap(ref int first, ref int second)
    {
        // TODO-00: Exchange the two values. `ref` passes the caller's variables, not copies.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool TryDivide(int dividend, int divisor, out int quotient, out int remainder)
    {
        // TODO-01: Return false when the divisor is zero, true otherwise. An `out` parameter must
        //   be assigned on every path before returning, even on failure (use 0).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static ref readonly Particle Heaviest(Particle[] particles)
    {
        // TODO-02: Return a read-only reference to the element with the largest Mass, not a copy.
        //   Use `return ref particles[index];`. The caller cannot assign through it.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int Sum(params ReadOnlySpan<int> values)
    {
        // TODO-03: Add all the values. A `params ReadOnlySpan<T>` (C# 13) lets callers pass
        //   `Sum(1, 2, 3)` without allocating an array.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string BuildUrl(string host, int port = 80, bool secure = false)
    {
        // TODO-04: Return "http://host:port", or "https://host:port" when secure is true.
        //   Defaults are copied into the caller at compile time, which is why changing one in a
        //   library requires recompiling every caller.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Pick(long value)
    {
        // TODO-05: Return "long". Before running the tests, predict which overload `Pick(5)` binds to.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Pick(double value)
    {
        // TODO-06: Return "double".
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string Pick(object value)
    {
        // TODO-07: Return "object".
        throw new NotImplementedException("Not implemented yet.");
    }
}
