namespace TaskBoard.Tests;

[TestClass]
public sealed class NewTaskFormTests : BunitContext
{
    private readonly List<NewTask> _created = [];

    private IRenderedComponent<NewTaskForm> RenderForm() =>
        Render<NewTaskForm>(p => p.Add(f => f.OnCreate, (NewTask task) => _created.Add(task)));

    [TestMethod]
    public void SubmittingWithoutATitleShowsTheMessageAndCreatesNothing()
    {
        var cut = RenderForm();

        cut.Find("form").Submit();

        Assert.Contains("Give the task a title.", cut.Markup);
        Assert.IsEmpty(_created);
    }

    [TestMethod]
    public void APriorityOutOfRangeIsRejected()
    {
        var cut = RenderForm();
        cut.Find("#title").Change("Buy milk");
        cut.Find("#priority").Change("9");

        cut.Find("form").Submit();

        Assert.Contains("Priority goes from 1 to 5.", cut.Markup);
        Assert.IsEmpty(_created);
    }

    [TestMethod]
    public void AValidSubmitRaisesOnCreateAndClearsTheForm()
    {
        var cut = RenderForm();
        cut.Find("#title").Change("Buy milk");
        cut.Find("#priority").Change("5");

        cut.Find("form").Submit();

        var created = Assert.ContainsSingle(_created);
        Assert.AreEqual("Buy milk", created.Title);
        Assert.AreEqual(5, created.Priority);
        Assert.AreEqual("", cut.Find("#title").GetAttribute("value"));
    }
}
