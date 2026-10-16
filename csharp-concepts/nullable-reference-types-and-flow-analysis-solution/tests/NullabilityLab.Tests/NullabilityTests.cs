using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NullabilityLab.Tests;

public class NullabilityTests
{
    // Nullable reference types are a compile-time feature, but the compiler records the
    // annotations in metadata. NullabilityInfoContext (public API) reads them back, and the
    // flow-analysis attributes are ordinary attributes, so reflection is enough to check them.
    private static readonly NullabilityInfoContext Context = new();

    private static MethodInfo Method(string name) => typeof(Nulls).GetMethod(name)!;

    private static NullabilityState ReturnState(string method) =>
        Context.Create(Method(method).ReturnParameter).ReadState;

    [Fact]
    public void ShouldMarkNicknameAsNullableAndEmailAsNotNull()
    {
        Assert.Equal(NullabilityState.Nullable, Context.Create(typeof(User).GetProperty(nameof(User.Nickname))!).ReadState);
        Assert.Equal(NullabilityState.NotNull, Context.Create(typeof(User).GetProperty(nameof(User.Email))!).ReadState);
    }

    [Fact]
    public void ShouldFallBackToEmailWhenThereIsNoNickname()
    {
        Assert.Equal("annie", Nulls.DisplayName(new User { Email = "a@x.com", Nickname = "annie" }));
        Assert.Equal("a@x.com", Nulls.DisplayName(new User { Email = "a@x.com" }));
        Assert.Equal("a@x.com", Nulls.DisplayName(new User { Email = "a@x.com", Nickname = "  " }));
    }

    [Fact]
    public void ShouldReturnNullForAnUnknownUser()
    {
        Assert.NotNull(Nulls.FindUser("1"));
        Assert.Null(Nulls.FindUser("999"));
    }

    [Fact]
    public void ShouldDeclareFindUserAsReturningNullable()
    {
        Assert.Equal(NullabilityState.Nullable, ReturnState(nameof(Nulls.FindUser)));
    }

    [Fact]
    public void ShouldTryGetAUser()
    {
        Assert.True(Nulls.TryGetUser("2", out var user));
        Assert.Equal("bob@example.com", user.Email);
        Assert.False(Nulls.TryGetUser("999", out _));
    }

    [Fact]
    public void ShouldAnnotateTryGetUserWithNotNullWhenTrue()
    {
        var parameter = Method(nameof(Nulls.TryGetUser)).GetParameters()[1];

        var attribute = parameter.GetCustomAttribute<NotNullWhenAttribute>();

        Assert.NotNull(attribute);
        Assert.True(attribute.ReturnValue);
    }

    [Fact]
    public void ShouldTrimAndKeepNull()
    {
        Assert.Equal("a b", Nulls.Trim("  a b "));
        Assert.Null(Nulls.Trim(null));
    }

    [Fact]
    public void ShouldAnnotateTrimWithNotNullIfNotNull()
    {
        var attribute = Method(nameof(Nulls.Trim)).ReturnParameter.GetCustomAttribute<NotNullIfNotNullAttribute>();

        Assert.NotNull(attribute);
        Assert.Equal("value", attribute.ParameterName);
    }

    [Fact]
    public void ShouldInitializeTheEndpointThroughAMemberNotNullMethod()
    {
        Assert.Equal("localhost:5432", new Connection().Endpoint);

        var initialize = typeof(Connection).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var attribute = initialize.GetCustomAttribute<MemberNotNullAttribute>();

        Assert.NotNull(attribute);
        Assert.Contains("_endpoint", attribute.Members);
    }

    [Fact]
    public void ShouldStillThrowAtRunTimeWhenANullSlipsPastTheCompiler()
    {
        Assert.Equal("ok", Nulls.Require("ok"));
        Assert.Throws<ArgumentNullException>(() => Nulls.Require(null!));
    }

    [Theory]
    [InlineData(1, 1, "equal")]
    [InlineData(null, null, "equal")]
    [InlineData(1, 2, "less")]
    [InlineData(3, 2, "greater")]
    [InlineData(null, 2, "unordered")]
    [InlineData(2, null, "unordered")]
    public void ShouldCompareNullableIntsWithLiftedOperators(int? a, int? b, string expected)
    {
        Assert.Equal(expected, Nulls.Compare(a, b));
    }

    [Fact]
    public void ShouldReturnNullableOfTForValueTypes()
    {
        Assert.Equal(7, Nulls.FirstOrNull([7, 8]));
        Assert.Null(Nulls.FirstOrNull(Array.Empty<int>()));
    }

    [Fact]
    public void ShouldConstrainFirstOrNullToStructs()
    {
        var parameter = Method(nameof(Nulls.FirstOrNull)).GetGenericArguments()[0];

        Assert.True(parameter.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint));
    }

    // Bonus tests (TODO-09 and TODO-10 exist only in the solution).

    [Fact]
    public void ShouldTryFindTheFirstMatch()
    {
        Assert.True(Nulls.TryFirst(["ant", "bee"], s => s.StartsWith('b'), out var found));
        Assert.Equal("bee", found);
        Assert.False(Nulls.TryFirst(["ant"], s => s.StartsWith('z'), out _));
    }

    [Fact]
    public void ShouldAnnotateTryFirstWithMaybeNullWhenFalse()
    {
        var parameter = Method(nameof(Nulls.TryFirst)).GetParameters()[2];

        var attribute = parameter.GetCustomAttribute<MaybeNullWhenAttribute>();

        Assert.NotNull(attribute);
        Assert.False(attribute.ReturnValue);
    }

    [Fact]
    public void ShouldCountItemsByANotNullKey()
    {
        var counts = Nulls.CountBy(["a", "b", "a"]);

        Assert.Equal(2, counts["a"]);
        Assert.Equal(1, counts["b"]);
    }
}
