namespace TaskBoard.Tests;

// BunitContext is bUnit's test context (it was called TestContext before bUnit 2, which clashed with
// MSTest's TestContext). MSTest creates one instance per test and disposes it afterwards.
[TestClass]
public sealed class CounterTests : BunitContext
{
    [TestMethod]
    public void ClickingTheButtonIncrementsTheCount()
    {
        var cut = Render<Counter>();

        cut.Find("button").Click();

        cut.Find("[role=status]").MarkupMatches("<p role=\"status\">Clicked 1 of 3</p>");
    }

    [TestMethod]
    public void AtTheLimitTheButtonIsDisabledAndTheCallbackGetsTheCount()
    {
        var reached = new List<int>();
        var cut = Render<Counter>(p => p
            .Add(c => c.Max, 2)
            .Add(c => c.OnLimitReached, (int count) => reached.Add(count)));

        cut.Find("button").Click();
        Assert.IsFalse(cut.Find("button").HasAttribute("disabled"));
        cut.Find("button").Click();

        Assert.IsTrue(cut.Find("button").HasAttribute("disabled"));
        Assert.AreEqual(2, Assert.ContainsSingle(reached));
    }
}
