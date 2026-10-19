namespace TaskBoard.Tests;

// BunitContext is bUnit's test context (it was called TestContext before bUnit 2, which clashed with
// MSTest's TestContext). MSTest creates one instance per test and disposes it afterwards.
// TODO-00: Read src/TaskBoard/Counter.razor, then write the two tests below.
[TestClass]
public sealed class CounterTests : BunitContext
{
    [TestMethod]
    public void ClickingTheButtonIncrementsTheCount()
    {
        // Render<Counter>(), Find the button, Click() it, then compare the status paragraph with MarkupMatches.
        Assert.Fail("TODO-00: write this test.");
    }

    [TestMethod]
    public void AtTheLimitTheButtonIsDisabledAndTheCallbackGetsTheCount()
    {
        // Pass Max = 2 and an OnLimitReached handler with Render<Counter>(p => p.Add(...).Add(...)).
        // Check the disabled attribute before and after the limit, and that the callback ran once, with 2.
        Assert.Fail("TODO-00: write this test.");
    }
}
