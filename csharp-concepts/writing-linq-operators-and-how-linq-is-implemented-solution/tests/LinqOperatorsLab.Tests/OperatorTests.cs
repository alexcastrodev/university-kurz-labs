namespace LinqOperatorsLab.Tests;

public class OperatorTests
{
    // TODO-00: MyWhere / MySelect

    [Fact]
    public void ShouldFailAtTheCallWhenArgumentsAreNull()
    {
        IEnumerable<int> nothing = null!;

        Assert.Throws<ArgumentNullException>(() => nothing.MyWhere(x => true));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.MyWhere(null!));
        Assert.Throws<ArgumentNullException>(() => nothing.MySelect(x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.MySelect<int, int>(null!));
    }

    [Fact]
    public void ShouldNotTouchTheSourceUntilEnumerated()
    {
        var source = new TrackedSource(5);

        var query = source.MyWhere(x => x % 2 == 1).MySelect(x => x * 10);

        Assert.Equal(0, source.Enumerations);
        Assert.Equal(0, source.Pulled);
        Assert.Equal([10, 30, 50], query.ToList());
    }

    [Fact]
    public void ShouldRunThePredicateOncePerElementAndReEvaluateEachEnumeration()
    {
        var source = new TrackedSource(4);
        var calls = 0;
        var minimum = 3;
        var query = source.MyWhere(x => { calls++; return x >= minimum; });

        Assert.Equal([3, 4], query.ToList());
        Assert.Equal(4, calls);

        minimum = 4;   // the closure sees the new value on the next enumeration
        Assert.Equal([4], query.ToList());
        Assert.Equal(2, source.Enumerations);
    }

    [Fact]
    public void ShouldPullOneItemAtATimeThroughTheWholePipeline()
    {
        var source = new TrackedSource(100);

        var first = source.MyWhere(x => x > 2).MySelect(x => x * 2).First();

        Assert.Equal(6, first);
        Assert.Equal(3, source.Pulled);
        Assert.Equal(1, source.Disposed);
    }

    // TODO-01: MyTake

    [Fact]
    public void ShouldNotPullMoreThanNeededNorTouchTheSourceForZero()
    {
        var source = new TrackedSource(10);

        Assert.Equal([1, 2, 3], source.MyTake(3).ToList());
        Assert.Equal(3, source.Pulled);   // no fourth MoveNext "just to check"

        var untouched = new TrackedSource(10);
        Assert.Empty(untouched.MyTake(0));
        Assert.Equal(0, untouched.Pulled);
    }

    [Fact]
    public void ShouldTakeEverythingWhenCountIsLargerThanTheSource()
    {
        Assert.Equal([1, 2], new TrackedSource(2).MyTake(50).ToList());
    }

    [Fact]
    public void ShouldValidateTakeEagerly()
    {
        IEnumerable<int> nothing = null!;

        Assert.Throws<ArgumentNullException>(() => nothing.MyTake(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.MyTake(-1));
    }

    [Fact]
    public void ShouldDisposeTheSourceWhenTheConsumerBreaksEarly()
    {
        var source = new TrackedSource(10);

        foreach (var item in source.MyTake(8))
        {
            if (item == 2) break;
        }

        Assert.Equal(2, source.Pulled);
        Assert.Equal(1, source.Disposed);
    }

    // TODO-02: MyDistinctBy

    [Fact]
    public void ShouldKeepTheFirstElementOfEachKey()
    {
        string[] words = ["apple", "avocado", "banana", "blueberry", "cherry", "apricot"];

        Assert.Equal(["apple", "banana", "cherry"], words.MyDistinctBy(w => w[0]).ToList());
    }

    [Fact]
    public void ShouldStreamAndRunTheKeySelectorOncePerElement()
    {
        var source = new TrackedSource(100);
        var keyCalls = 0;

        var firstThree = source.MyDistinctBy(x => { keyCalls++; return x % 3; }).Take(3).ToList();

        Assert.Equal([1, 2, 3], firstThree);
        Assert.Equal(3, source.Pulled);
        Assert.Equal(3, keyCalls);
    }

    [Fact]
    public void ShouldStartWithAnEmptySeenSetOnEveryEnumeration()
    {
        var query = new[] { 1, 2, 3, 4 }.MyDistinctBy(x => x % 2);

        Assert.Equal([1, 2], query.ToList());
        Assert.Equal([1, 2], query.ToList());
    }

    [Fact]
    public void ShouldValidateDistinctByEagerly()
    {
        IEnumerable<int> nothing = null!;

        Assert.Throws<ArgumentNullException>(() => nothing.MyDistinctBy(x => x));
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.MyDistinctBy<int, int>(null!));
    }

    // TODO-03: MyChunk

    [Fact]
    public void ShouldSplitIntoChunksWithAShorterLastOne()
    {
        var chunks = new TrackedSource(7).MyChunk(3).ToList();

        Assert.Equal(3, chunks.Count);
        Assert.Equal([1, 2, 3], chunks[0]);
        Assert.Equal([4, 5, 6], chunks[1]);
        Assert.Equal([7], chunks[2]);
    }

    [Fact]
    public void ShouldNotProduceAnEmptyTrailingChunk()
    {
        Assert.Equal(2, new TrackedSource(6).MyChunk(3).Count());
        Assert.Empty(new TrackedSource(0).MyChunk(3));
    }

    [Fact]
    public void ShouldPullOnlyTheFirstChunkAndDisposeWhenStoppingEarly()
    {
        var source = new TrackedSource(100);

        var first = source.MyChunk(4).First();

        Assert.Equal([1, 2, 3, 4], first);
        Assert.Equal(4, source.Pulled);
        Assert.Equal(1, source.Disposed);
    }

    [Fact]
    public void ShouldValidateChunkSizeEagerly()
    {
        IEnumerable<int> nothing = null!;

        Assert.Throws<ArgumentNullException>(() => nothing.MyChunk(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.MyChunk(0));
    }

    [Fact]
    public void ShouldYieldIndependentArrays()
    {
        var chunks = new[] { 1, 2, 3, 4 }.MyChunk(2).ToList();

        chunks[0][0] = 99;

        Assert.Equal([3, 4], chunks[1]);
        Assert.Equal([99, 2], chunks[0]);
    }

    // TODO-04: MyCount / MyElementAt fast paths

    [Fact]
    public void ShouldCountACollectionWithoutEnumeratingIt()
    {
        var spy = new ListSpy(5, 6, 7);

        Assert.Equal(3, spy.MyCount());
        Assert.Equal(0, spy.EnumeratorsCreated);
        Assert.Equal(3, new[] { 1, 2, 3 }.MyCount());
    }

    [Fact]
    public void ShouldCountAnIteratorByEnumeratingAndDisposing()
    {
        var source = new TrackedSource(6);

        Assert.Equal(6, source.MyCount());
        Assert.Equal(6, source.Pulled);
        Assert.Equal(1, source.Disposed);
    }

    [Fact]
    public void ShouldIndexAListWithoutEnumeratingIt()
    {
        var spy = new ListSpy(5, 6, 7);

        Assert.Equal(7, spy.MyElementAt(2));
        Assert.Equal(0, spy.EnumeratorsCreated);
    }

    [Fact]
    public void ShouldStopPullingAtTheIndexForAnIterator()
    {
        var source = new TrackedSource(100);

        Assert.Equal(4, source.MyElementAt(3));
        Assert.Equal(4, source.Pulled);
        Assert.Equal(1, source.Disposed);
    }

    [Fact]
    public void ShouldRejectOutOfRangeIndexesOnBothPaths()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ListSpy(1, 2).MyElementAt(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new ListSpy(1, 2).MyElementAt(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TrackedSource(2).MyElementAt(2));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TrackedSource(2).MyElementAt(-1));
    }
}
