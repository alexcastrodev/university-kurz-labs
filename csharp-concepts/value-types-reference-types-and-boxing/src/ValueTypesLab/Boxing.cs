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
        // TODO-04: Return the ints stored in the boxes. Unboxing is an explicit cast, (int).
        throw new NotImplementedException("Not implemented yet.");
    }

    public static long SumOnStack(int count)
    {
        // TODO-05: Put the numbers 1..count in a Span<int> created with stackalloc and return
        //   their sum. No int[] and no List<int>: the buffer must not touch the heap.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static object? BoxNullable(int? value)
    {
        // TODO-06: Return the value as object. Run the tests and note what a Nullable<int>
        //   without a value becomes, and what a Nullable<int> with a value becomes.
        throw new NotImplementedException("Not implemented yet.");
    }
}
