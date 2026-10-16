using System.Reflection;

namespace GenericsLab.Tests;

public class GenericsTests
{
    private struct MarkerA;
    private struct MarkerB;

    private static GenericParameterAttributes Attributes(string method) =>
        typeof(Generics).GetMethod(method)!.GetGenericArguments()[0].GenericParameterAttributes;

    [Fact]
    public void ShouldFindTheMaximumOfNumbersAndStrings()
    {
        Assert.Equal(9, Generics.Max([3, 9, 4]));
        Assert.Equal("pear", Generics.Max(["apple", "pear", "fig"]));
    }

    [Fact]
    public void ShouldThrowWhenTheSequenceIsEmpty()
    {
        Assert.Throws<InvalidOperationException>(() => Generics.Max(Array.Empty<int>()));
    }

    [Fact]
    public void ShouldConstrainMaxToIComparableOfT()
    {
        var parameter = typeof(Generics).GetMethod(nameof(Generics.Max))!.GetGenericArguments()[0];

        var constraint = Assert.Single(parameter.GetGenericParameterConstraints());
        Assert.Equal(typeof(IComparable<>), constraint.GetGenericTypeDefinition());
    }

    [Fact]
    public void ShouldCreateAnInstanceOfAClassWithADefaultConstructor()
    {
        Assert.NotNull(Generics.Create<List<int>>());
    }

    [Fact]
    public void ShouldConstrainCreateToClassWithNew()
    {
        var attributes = Attributes(nameof(Generics.Create));

        Assert.True(attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint));
        Assert.True(attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint));
    }

    [Fact]
    public void ShouldCountTheBytesOfAnUnmanagedValue()
    {
        Assert.Equal(8, Generics.ByteCount(42L));
        Assert.Equal(4, Generics.ByteCount(42));
        Assert.Equal(16, Generics.ByteCount(Guid.NewGuid()));
    }

    [Fact]
    public void ShouldConstrainByteCountToValueTypes()
    {
        Assert.True(Attributes(nameof(Generics.ByteCount)).HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint));
    }

    [Fact]
    public void ShouldDescribeTheTypeArgumentAndItsDefault()
    {
        Assert.Equal("Int32:0", Generics.Describe<int>());
        Assert.Equal("String:null", Generics.Describe<string>());
    }

    [Fact]
    public void ShouldKeepOneCounterPerClosedType()
    {
        TypeCounter<MarkerA>.Increment();
        TypeCounter<MarkerA>.Increment();
        TypeCounter<MarkerB>.Increment();

        Assert.Equal(2, TypeCounter<MarkerA>.Hits);
        Assert.Equal(1, TypeCounter<MarkerB>.Hits);
    }

    [Fact]
    public void ShouldTreatAProducerOfDogsAsAProducerOfAnimals()
    {
        Assert.True(typeof(IProducer<Animal>).IsAssignableFrom(typeof(IProducer<Dog>)));
        Assert.True(typeof(IProducer<Animal>).IsAssignableFrom(typeof(Kennel)));
    }

    [Fact]
    public void ShouldTreatAConsumerOfAnimalsAsAConsumerOfDogs()
    {
        Assert.True(typeof(IConsumer<Dog>).IsAssignableFrom(typeof(IConsumer<Animal>)));
        Assert.True(typeof(IConsumer<Dog>).IsAssignableFrom(typeof(AnimalLog)));
    }

    [Fact]
    public void ShouldNotTreatAListOfDogsAsAListOfAnimals()
    {
        Assert.False(typeof(List<Animal>).IsAssignableFrom(typeof(List<Dog>)));
    }

    [Fact]
    public void ShouldCompareWithoutBoxingWhenTheConstraintIsUsed()
    {
        for (var i = 0; i < 1_000; i++) Generics.CompareViaConstraint(i, 5);

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10_000; i++) Generics.CompareViaConstraint(i, 5);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0, allocated);
    }

    [Fact]
    public void ShouldBoxWhenTheValueIsPassedAsAnInterface()
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 10_000; i++) Generics.CompareViaInterface(i, 5);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.True(allocated >= 10_000 * 24);
    }
}
