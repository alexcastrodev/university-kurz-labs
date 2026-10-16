using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using FieldsLab;

namespace FieldsLab.Tests;

public class FieldsTests
{
    [Fact]
    public void BaseConstructorShouldSeeADerivedFieldInitializerButNotTheConstructorAssignment() =>
        Assert.Equal("name=initialized;late=null", new Derived().SeenByBase);

    [Fact]
    public void AfterConstructionBothFieldsAreSet() =>
        Assert.Equal("name=initialized;late=set", new Derived().Now());

    [Fact]
    public void MaxRetriesShouldBeAConst()
    {
        var field = typeof(Limits).GetField(nameof(Limits.MaxRetries))!;

        Assert.True(field.IsLiteral);
    }

    [Fact]
    public void TimeoutShouldBeStaticReadonly()
    {
        var field = typeof(Limits).GetField(nameof(Limits.Timeout))!;

        Assert.True(field.IsStatic);
        Assert.True(field.IsInitOnly);
    }

    [Fact]
    public void PreciseTypeShouldNotBeBeforeFieldInit() =>
        Assert.False(typeof(Precise).Attributes.HasFlag(TypeAttributes.BeforeFieldInit));

    [Fact]
    public void RelaxedTypeShouldBeBeforeFieldInit() =>
        Assert.True(typeof(Relaxed).Attributes.HasFlag(TypeAttributes.BeforeFieldInit));

    [Theory]
    [InlineData(nameof(Person.Name))]
    [InlineData(nameof(Person.Born))]
    public void PersonMembersShouldBeRequired(string property) =>
        Assert.NotNull(typeof(Person).GetProperty(property)!.GetCustomAttribute<RequiredMemberAttribute>());

    [Fact]
    public void TheConstructorThatAssignsEverythingShouldVouchForIt()
    {
        var ctor = typeof(Person).GetConstructor([typeof(string), typeof(int)])!;

        Assert.NotNull(ctor.GetCustomAttribute<SetsRequiredMembersAttribute>());
    }

    [Fact]
    public void PersonShouldBeBuiltByInitializerOrConstructor()
    {
        var a = new Person { Name = "Ana", Born = 1990 };
        var b = new Person("Bia", 1985);

        Assert.Equal(("Ana", 1990), (a.Name, a.Born));
        Assert.Equal(("Bia", 1985), (b.Name, b.Born));
    }
}
