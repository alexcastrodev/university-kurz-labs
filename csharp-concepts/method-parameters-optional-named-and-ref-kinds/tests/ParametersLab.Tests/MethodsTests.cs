using System.Runtime.CompilerServices;
using ParametersLab;

namespace ParametersLab.Tests;

public class MethodsTests
{
    [Fact]
    public void ShouldSwapTheCallersVariables()
    {
        var left = 1;
        var right = 2;

        Methods.Swap(ref left, ref right);

        Assert.Equal(2, left);
        Assert.Equal(1, right);
    }

    [Fact]
    public void ShouldReturnQuotientAndRemainder()
    {
        Assert.True(Methods.TryDivide(17, 5, out var quotient, out var remainder));
        Assert.Equal(3, quotient);
        Assert.Equal(2, remainder);
    }

    [Fact]
    public void ShouldFailWithZeroOutputsWhenDividingByZero()
    {
        Assert.False(Methods.TryDivide(17, 0, out var quotient, out var remainder));
        Assert.Equal(0, quotient);
        Assert.Equal(0, remainder);
    }

    [Fact]
    public void ShouldReturnAReferenceToTheHeaviestElement()
    {
        var particles = new[] { new Particle("a", 1), new Particle("b", 9), new Particle("c", 3) };

        ref readonly var heaviest = ref Methods.Heaviest(particles);

        Assert.Equal("b", heaviest.Name);
        Assert.True(Unsafe.AreSame(ref Unsafe.AsRef(in heaviest), ref particles[1]));
    }

    [Fact]
    public void ShouldSumAnyNumberOfArguments()
    {
        Assert.Equal(0, Methods.Sum());
        Assert.Equal(6, Methods.Sum(1, 2, 3));
        Assert.Equal(10, Methods.Sum([1, 2, 3, 4]));
    }

    [Fact]
    public void ShouldUseTheDefaultsWhenOmitted() =>
        Assert.Equal("http://example.com:80", Methods.BuildUrl("example.com"));

    [Fact]
    public void ShouldAcceptNamedArgumentsInAnyOrder() =>
        Assert.Equal("https://example.com:80", Methods.BuildUrl(secure: true, host: "example.com"));

    [Fact]
    public void ShouldSkipAnOptionalParameterByNamingTheNextOne() =>
        Assert.Equal("https://example.com:80", Methods.BuildUrl("example.com", secure: true));

    [Fact]
    public void ShouldPreferTheBetterConversionInOverloadResolution()
    {
        Assert.Equal("long", Methods.Pick(5));
        Assert.Equal("double", Methods.Pick(5f));
        Assert.Equal("object", Methods.Pick("five"));
    }
}
