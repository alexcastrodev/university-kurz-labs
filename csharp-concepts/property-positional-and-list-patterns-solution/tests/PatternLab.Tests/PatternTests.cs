namespace PatternLab.Tests;

public class PatternTests
{
    [Fact]
    public void ShouldMatchRegularLisbonCustomers()
    {
        Assert.True(Patterns.IsLisbonRegular(new Customer("Ana", new Address("Lisbon", "PT"), 3)));
        Assert.False(Patterns.IsLisbonRegular(new Customer("Ana", new Address("Lisbon", "PT"), 0)));
        Assert.False(Patterns.IsLisbonRegular(new Customer("Ana", new Address("Porto", "PT"), 3)));
    }

    [Fact]
    public void ShouldAnswerFalseWithoutAnAddress() =>
        Assert.False(Patterns.IsLisbonRegular(new Customer("Bia", null, 5)));

    [Theory]
    [InlineData(0, 0, "origin")]
    [InlineData(3, 0, "x-axis")]
    [InlineData(0, -2, "y-axis")]
    [InlineData(1, 1, "Q1")]
    [InlineData(-1, 1, "Q2")]
    [InlineData(-1, -1, "Q3")]
    [InlineData(1, -1, "Q4")]
    public void ShouldClassifyPoints(int x, int y, string expected) =>
        Assert.Equal(expected, Patterns.Quadrant(new Point(x, y)));

    [Theory]
    [InlineData(State.Closed, Trigger.Open, State.Open)]
    [InlineData(State.Open, Trigger.Close, State.Closed)]
    [InlineData(State.Closed, Trigger.Lock, State.Locked)]
    [InlineData(State.Locked, Trigger.Unlock, State.Closed)]
    public void ShouldFollowValidTransitions(State state, Trigger trigger, State expected) =>
        Assert.Equal(expected, Patterns.Next(state, trigger));

    [Fact]
    public void ShouldRejectInvalidTransitions()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => Patterns.Next(State.Open, Trigger.Lock));

        Assert.Equal("Lock is not valid in Open", ex.Message);
    }

    [Fact]
    public void ShouldDescribeSequencesByShape()
    {
        Assert.Equal("empty", Patterns.Describe([]));
        Assert.Equal("one: 7", Patterns.Describe([7]));
        Assert.Equal("first 1, rest [2,3,4]", Patterns.Describe([1, 2, 3, 4]));
    }

    [Theory]
    [InlineData("(abc)", "abc")]
    [InlineData("()", "")]
    [InlineData("(abc", null)]
    [InlineData(")", null)]
    [InlineData("", null)]
    public void ShouldExtractTheTextBetweenParentheses(string text, string? expected) =>
        Assert.Equal(expected, Patterns.Inner(text));

    [Theory]
    [InlineData("move 3 4", "move by (3, 4)")]
    [InlineData("  move   3   4 ", "move by (3, 4)")]
    [InlineData("say hello world", "say 'hello world'")]
    [InlineData("quit", "bye")]
    [InlineData("exit", "bye")]
    [InlineData("jump", "unknown command 'jump'")]
    [InlineData("move x y", "unknown command 'move'")]
    [InlineData("move 1", "unknown command 'move'")]
    [InlineData("   ", "empty line")]
    public void ShouldParseCommands(string line, string expected) =>
        Assert.Equal(expected, Patterns.Parse(line));

    [Fact]
    public void ShouldRecognizeTheOriginInAnyShape()
    {
        Assert.True(Patterns.IsOrigin((0, 0)));
        Assert.True(Patterns.IsOrigin(new Point(0, 0)));
        Assert.False(Patterns.IsOrigin((0, 1)));
        Assert.False(Patterns.IsOrigin(new Point(1, 0)));
        Assert.False(Patterns.IsOrigin("origin"));
    }
}
