using System.Runtime;

namespace GcLab;

public static class Heap
{
    public static int GenerationOfNewByteArray(int length)
    {
        // TODO-00: Allocate a new byte[length] and return its generation with GC.GetGeneration.
        //   Objects of 85,000 bytes or more (header included) go to the large object heap,
        //   which is logically part of generation 2, so they report 2 right away.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int[] GenerationsAfterCollections(object value)
    {
        // TODO-01: Return the generation of `value` measured after GC.Collect(0), then after
        //   GC.Collect(1), then after GC.Collect(2), in this order. A survivor moves up one
        //   generation per collection of its generation, until generation 2.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int CollectionsTriggeredBy(int generation)
    {
        // TODO-02: Throw ArgumentOutOfRangeException when `generation` is negative or greater
        //   than GC.MaxGeneration. Otherwise run GC.Collect(generation) and return how much
        //   GC.CollectionCount(generation) grew because of it.
        throw new NotImplementedException("Not implemented yet.");
    }

    // Bonus

    public static GCLargeObjectHeapCompactionMode RequestLohCompaction()
    {
        // TODO-06 (optional): Set GCSettings.LargeObjectHeapCompactionMode to CompactOnce, run a
        //   blocking GC.Collect(), and return the mode read back afterwards.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int GenerationOfPinnedArray(int length)
    {
        // TODO-07 (optional): Allocate a byte array on the pinned object heap with
        //   GC.AllocateArray<byte>(length, pinned: true) and return its generation.
        throw new NotImplementedException("Not implemented yet.");
    }
}
