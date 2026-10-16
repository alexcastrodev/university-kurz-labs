using System.Runtime.InteropServices;

namespace ValueTypesLab;

public static class Semantics
{
    public static int IncrementCopy(Counter counter)
    {
        counter.Increment();
        return counter.Value;
    }

    public static void IncrementRef(ref Counter counter) => counter.Increment();

    public static double Length(in Vector3D vector) =>
        Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);

    public static void IncrementFirst(List<Counter> counters) =>
        CollectionsMarshal.AsSpan(counters)[0].Increment();
}
