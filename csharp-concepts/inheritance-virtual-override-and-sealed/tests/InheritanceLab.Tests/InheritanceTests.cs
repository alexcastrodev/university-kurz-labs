using System.Reflection;
using InheritanceLab;

namespace InheritanceLab.Tests;

public class InheritanceTests
{
    [Fact]
    public void OverrideShouldBePickedByTheRuntimeType()
    {
        Animal a = new Dog();

        Assert.Equal("Dog.Speak > Animal.Speak", a.Speak());
    }

    [Fact]
    public void HidingShouldBePickedByTheStaticType()
    {
        Animal a = new Dog();
        Dog d = (Dog)a;

        Assert.Equal("Animal.Describe", a.Describe());
        Assert.Equal("Dog.Describe", d.Describe());
    }

    [Fact]
    public void CloneShouldDispatchVirtuallyAndReturnADog()
    {
        Animal a = new Dog();

        Assert.IsType<Dog>(a.Clone());
    }

    [Fact]
    public void CloneShouldHaveACovariantReturnType()
    {
        var clone = typeof(Dog).GetMethod(nameof(Dog.Clone), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, Type.EmptyTypes);

        Assert.NotNull(clone);
        Assert.Equal(typeof(Dog), clone.ReturnType);
    }

    [Fact]
    public void ReportShouldBeAbstractWithANonPublicConstructor()
    {
        Assert.True(typeof(Report).IsAbstract);
        Assert.All(
            typeof(Report).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            c => Assert.False(c.IsPublic));
    }

    [Fact]
    public void SalesReportShouldBeSealedAndRender()
    {
        Assert.True(typeof(SalesReport).IsSealed);
        Assert.Equal("Sales @ 2026-10-14", new SalesReport().Render());
    }

    [Theory]
    [InlineData(11.0, "big circle")]
    [InlineData(2.0, "circle 2")]
    public void ShouldDescribeCircles(double radius, string expected) =>
        Assert.Equal(expected, ShapeText.Describe(new Circle(radius)));

    [Fact]
    public void ShouldDescribeSquaresAndUnknowns()
    {
        Assert.Equal("square 3", ShapeText.Describe(new Square(3)));
        Assert.Equal("unknown", ShapeText.Describe(null));
    }

    [Fact]
    public void CountingBagShouldCountEachItemOnce()
    {
        var bag = new CountingBag();

        bag.AddRange(["a", "b", "c"]);
        Assert.Equal(3, bag.Added);

        bag.Add("d");
        Assert.Equal((4, 4), (bag.Added, bag.Count));
    }

    [Fact]
    public void CountingBagShouldUseCompositionNotInheritance() =>
        Assert.Equal(typeof(object), typeof(CountingBag).BaseType);
}
