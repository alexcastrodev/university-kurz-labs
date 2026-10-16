using ClosuresLab;

namespace ClosuresLab.Tests;

public class BonusTests
{
    [Theory]
    [InlineData(false, "Hello, Ana")]
    [InlineData(true, "Good evening, Ana")]
    public void ShouldUseTheLambdaDefaultParameter(bool formal, string expected) =>
        Assert.Equal(expected, Functions.Greet("Ana", formal));

    [Theory]
    [InlineData("42", 42)]
    [InlineData("x", -1)]
    public void ShouldParseThroughALambdaWithAnOutParameter(string text, int expected) =>
        Assert.Equal(expected, Functions.ParseOrDefault(text, -1));

    [Fact]
    public void ShouldIncrementWithAStaticLambda() =>
        Assert.Equal(6, Functions.Increment()(5));
}
