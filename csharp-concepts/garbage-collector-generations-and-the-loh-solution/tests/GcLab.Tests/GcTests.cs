using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace GcLab.Tests;

// Every GC-sensitive test lives in this single class, and parallelization is off,
// so no other test can collect or allocate while one of them is measuring.
public class GcTests
{
    private static void FullCollect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference<object> CreateGarbage() => Weak.Of(new object());

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static (WeakReference Short, WeakReference Long) CreateResurrectable()
    {
        Resurrector.Saved = null;
        return Resurrector.Track(new Resurrector());
    }

    // TODO-00
    [Fact]
    public void ShouldPlaceBigArraysInGeneration2Immediately()
    {
        Assert.Equal(2, Heap.GenerationOfNewByteArray(1_000_000));
        Assert.Equal(2, Heap.GenerationOfNewByteArray(100_000));
    }

    [Fact]
    public void ShouldStartSmallArraysBelowGeneration2()
    {
        Assert.True(Heap.GenerationOfNewByteArray(1_000) < 2);
    }

    // TODO-01
    [Fact]
    public void ShouldPromoteOneGenerationPerSurvivedCollection()
    {
        var value = new object();

        var generations = Heap.GenerationsAfterCollections(value);

        Assert.True(generations[0] >= 1);
        Assert.Equal(2, generations[1]);
        Assert.Equal(2, generations[2]);
        GC.KeepAlive(value);
    }

    // TODO-02
    [Fact]
    public void ShouldCountACollectionForEveryGeneration()
    {
        Assert.Equal(2, GC.MaxGeneration);
        for (var generation = 0; generation <= GC.MaxGeneration; generation++)
        {
            Assert.True(Heap.CollectionsTriggeredBy(generation) >= 1);
        }
    }

    [Fact]
    public void ShouldRejectGenerationsThatDoNotExist()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Heap.CollectionsTriggeredBy(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Heap.CollectionsTriggeredBy(GC.MaxGeneration + 1));
    }

    // TODO-03
    [Fact]
    public void ShouldClearWeakReferenceWhenTargetIsGarbage()
    {
        var weak = CreateGarbage();

        FullCollect();

        Assert.False(Weak.IsAlive(weak));
    }

    [Fact]
    public void ShouldKeepWeakReferenceWhileStronglyReferenced()
    {
        var strong = new object();
        var weak = Weak.Of(strong);

        FullCollect();

        Assert.True(Weak.IsAlive(weak));
        GC.KeepAlive(strong);
    }

    // TODO-04
    [Fact]
    public void ShouldClearShortWeakReferenceBeforeTheFinalizerRuns()
    {
        var (shortRef, _) = CreateResurrectable();

        FullCollect();

        Assert.False(shortRef.IsAlive);
    }

    [Fact]
    public void ShouldKeepLongWeakReferenceWhenTheFinalizerResurrects()
    {
        var (_, longRef) = CreateResurrectable();

        FullCollect();

        Assert.NotNull(Resurrector.Saved);
        Assert.True(longRef.IsAlive);
        Assert.Same(Resurrector.Saved, longRef.Target);
        Resurrector.Saved = null;
    }

    // TODO-05
    [Fact]
    public void ShouldReturnWhatTheWorkFunctionComputes()
    {
        var result = Buffers.UsePooledArray(1_000, buffer => buffer.Length >= 1_000 ? 7 : -1);

        Assert.Equal(7, result);
    }

    [Fact]
    public void ShouldReturnTheBufferEvenWhenTheWorkThrows()
    {
        byte[]? first = null;
        Assert.Throws<InvalidOperationException>(() =>
            Buffers.UsePooledArray(1_000, buffer => { first = buffer; throw new InvalidOperationException(); }));

        byte[]? second = null;
        Buffers.UsePooledArray(1_000, buffer => { second = buffer; return 0; });

        // The shared pool keeps a per-thread cache, so the same array comes back.
        Assert.Same(first, second);
    }

    [Fact]
    public void ShouldAvoidAllocatingOneLargeArrayPerCall()
    {
        const int size = 1 << 20;
        const int calls = 50;
        Buffers.UsePooledArray(size, _ => 0); // warm up the pool
        Buffers.UseNewArray(size, _ => 0);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < calls; i++) Buffers.UsePooledArray(size, _ => 0);
        var pooled = GC.GetAllocatedBytesForCurrentThread() - before;

        before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < calls; i++) Buffers.UseNewArray(size, _ => 0);
        var fresh = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(fresh >= (long)size * calls);
        Assert.True(pooled < size);
    }

    // TODO-06 (bonus)
    [Fact]
    public void ShouldResetCompactionModeAfterTheCollection()
    {
        var modeAfter = Heap.RequestLohCompaction();

        Assert.Equal(GCLargeObjectHeapCompactionMode.Default, modeAfter);
    }

    // TODO-07 (bonus)
    [Fact]
    public void ShouldReportPinnedArraysAsGeneration2()
    {
        Assert.Equal(2, Heap.GenerationOfPinnedArray(100));
    }
}
