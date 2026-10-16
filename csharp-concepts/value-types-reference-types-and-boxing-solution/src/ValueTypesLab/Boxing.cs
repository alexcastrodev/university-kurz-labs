namespace ValueTypesLab;

public static class Boxing
{
    public static object[] BoxAll(int[] values)
    {
        var boxes = new object[values.Length];
        for (var i = 0; i < values.Length; i++) boxes[i] = values[i];
        return boxes;
    }

    public static int[] UnboxAll(object[] boxes)
    {
        var values = new int[boxes.Length];
        for (var i = 0; i < boxes.Length; i++) values[i] = (int)boxes[i];
        return values;
    }

    public static long SumOnStack(int count)
    {
        Span<int> numbers = stackalloc int[count];
        for (var i = 0; i < numbers.Length; i++) numbers[i] = i + 1;

        long sum = 0;
        foreach (var number in numbers) sum += number;
        return sum;
    }

    public static object? BoxNullable(int? value) => value;

    public static long AllocatedByFormat(int iterations)
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < iterations; i++) _ = string.Format("{0}", i);
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    public static long AllocatedByInterpolation(int iterations)
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < iterations; i++) _ = $"{i}";
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }
}
