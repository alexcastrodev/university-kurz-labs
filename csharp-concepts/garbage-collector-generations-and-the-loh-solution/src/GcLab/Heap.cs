using System.Runtime;

namespace GcLab;

public static class Heap
{
    public static int GenerationOfNewByteArray(int length)
    {
        return GC.GetGeneration(new byte[length]);
    }

    public static int[] GenerationsAfterCollections(object value)
    {
        GC.Collect(0);
        var afterGen0 = GC.GetGeneration(value);
        GC.Collect(1);
        var afterGen1 = GC.GetGeneration(value);
        GC.Collect(2);
        var afterGen2 = GC.GetGeneration(value);
        return [afterGen0, afterGen1, afterGen2];
    }

    public static int CollectionsTriggeredBy(int generation)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(generation, GC.MaxGeneration);
        ArgumentOutOfRangeException.ThrowIfNegative(generation);
        var before = GC.CollectionCount(generation);
        GC.Collect(generation);
        return GC.CollectionCount(generation) - before;
    }

    // Bonus

    public static GCLargeObjectHeapCompactionMode RequestLohCompaction()
    {
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
        return GCSettings.LargeObjectHeapCompactionMode;
    }

    public static int GenerationOfPinnedArray(int length)
    {
        return GC.GetGeneration(GC.AllocateArray<byte>(length, pinned: true));
    }
}
