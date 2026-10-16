using ExtensionLab;

namespace ExtensionLab.Tests;

public class ExtensionTests
{
    [Fact]
    public void IsEmptyShouldBeAnInstancePropertyOnAnySequence()
    {
        int[] empty = [];
        List<string> names = ["ana", "bia"];

        Assert.True(empty.IsEmpty);
        Assert.False(names.IsEmpty);
    }

    [Fact]
    public void JoinWithShouldJoinTheElements()
    {
        List<string> names = ["ana", "bia"];

        Assert.Equal("ana + bia", names.JoinWith(" + "));
    }

    [Fact]
    public void ShouldCallTheImplementationMethodThroughItsClass()
    {
        List<string> names = ["ana", "bia"];

        Assert.Equal("ana,bia", SequenceExtensions.JoinWith(names, ","));
    }

    [Fact]
    public void ShouldExtendAStaticTypeWithAStaticMember() =>
        Assert.Equal([1, 2, 3], IEnumerable<int>.Sequence(1, 3));

    [Fact]
    public void ShouldAddAStaticPropertyToAnotherTypeYouDoNotOwn() =>
        Assert.Equal(new Money(0), Money.Zero);

    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("  ", true)]
    [InlineData("x", false)]
    public void IsBlankShouldAcceptANullReceiver(string? text, bool expected) =>
        Assert.Equal(expected, text.IsBlank);

    [Fact]
    public void IncrementShouldChangeTheCallersVariable()
    {
        var counter = 41;

        counter.Increment();

        Assert.Equal(42, counter);
    }
}
