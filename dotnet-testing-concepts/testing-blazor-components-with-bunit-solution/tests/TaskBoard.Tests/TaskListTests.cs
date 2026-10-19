using AngleSharp.Dom;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace TaskBoard.Tests;

[TestClass]
public sealed class TaskListTests : BunitContext
{
    private readonly TaskItem _milk = new(Guid.NewGuid(), "Buy milk", 3);
    private readonly TaskItem _dog = new(Guid.NewGuid(), "Walk the dog", 2);

    private readonly BunitAuthorizationContext _auth;

    // TaskList contains an <AuthorizeView>, which needs the authentication state that a real app
    // cascades from the router. Every test renders TaskList, so every test needs AddAuthorization.
    public TaskListTests() => _auth = AddAuthorization();

    private IRenderedComponent<TaskList> RenderList(FakeTaskStore store, BoardSettings? settings = null)
    {
        Services.AddSingleton<ITaskStore>(store);
        return settings is null
            ? Render<TaskList>()
            : Render<TaskList>(p => p.AddCascadingValue(settings));
    }

    private static IElement ButtonWithText(IRenderedComponent<TaskList> cut, string text) =>
        cut.FindAll("button").Single(b => b.TextContent.Trim() == text);

    [TestMethod]
    public void ShowsLoadingUntilTheStoreAnswers()
    {
        var store = new FakeTaskStore(_milk, _dog) { HoldLoading = true };
        var cut = RenderList(store);
        Assert.Contains("Loading tasks...", cut.Markup);

        store.Release();

        cut.WaitForAssertion(() => Assert.HasCount(2, cut.FindAll("li")));
    }

    [TestMethod]
    public void AnEmptyStoreShowsTheEmptyMessage()
    {
        var store = new FakeTaskStore { HoldLoading = true };
        var cut = RenderList(store);

        store.Release();

        // Asserting "no <li>" right away would pass while the list is still loading.
        cut.WaitForState(() => cut.Markup.Contains("No tasks yet."));
        Assert.IsEmpty(cut.FindAll("li"));
    }

    [TestMethod]
    public void TickingTheCheckboxMarksTheTaskDoneAndSavesIt()
    {
        var milk = new TaskItem(Guid.NewGuid(), "Buy milk", 3);
        var store = new FakeTaskStore(milk);
        var cut = RenderList(store);

        cut.Find("input[aria-label='Done: Buy milk']").Change(true);

        Assert.IsTrue(milk.Done);
        Assert.AreEqual(1, store.Saves);
        Assert.IsTrue(cut.Find("input[aria-label='Done: Buy milk']").HasAttribute("checked"));
    }

    [TestMethod]
    public void ConfirmingTheDeleteRemovesTheTask()
    {
        var store = new FakeTaskStore(_milk, _dog);
        JSInterop.Setup<bool>("confirm", "Delete 'Buy milk'?").SetResult(true);
        var cut = RenderList(store);

        cut.Find("button[aria-label='Delete Buy milk']").Click();

        Assert.AreEqual("Walk the dog", Assert.ContainsSingle(cut.FindAll("li span")).TextContent);
        Assert.AreEqual(_dog, Assert.ContainsSingle(store.Items));
    }

    [TestMethod]
    public void DecliningTheDeleteKeepsTheTask()
    {
        var store = new FakeTaskStore(_milk, _dog);
        JSInterop.Setup<bool>("confirm", "Delete 'Buy milk'?").SetResult(false);
        var cut = RenderList(store);

        cut.Find("button[aria-label='Delete Buy milk']").Click();

        Assert.HasCount(2, cut.FindAll("li"));
        Assert.HasCount(2, store.Items);
    }

    [TestMethod]
    public void AddingATaskSavesItShowsItAndAnnouncesIt()
    {
        var store = new FakeTaskStore();
        JSInterop.SetupVoid("taskBoard.announce", "Added Walk the dog");
        var cut = RenderList(store);

        cut.Find("#title").Change("  Walk the dog ");
        cut.Find("form").Submit();

        Assert.AreEqual("Walk the dog", Assert.ContainsSingle(store.Items).Title);
        Assert.AreEqual("Walk the dog", cut.Find("li span").TextContent);
        JSInterop.VerifyInvoke("taskBoard.announce");
    }

    [TestMethod]
    public void HideCompletedFromTheCascadingSettingsHidesDoneTasks()
    {
        var done = new TaskItem(Guid.NewGuid(), "Pay rent", 1) { Done = true };
        var cut = RenderList(new FakeTaskStore(done, _dog), new BoardSettings(HideCompleted: true));

        Assert.AreEqual("Walk the dog", Assert.ContainsSingle(cut.FindAll("li span")).TextContent);
    }

    [TestMethod]
    public void OnlyALeadSeesClearCompleted()
    {
        _auth.SetAuthorized("bob");
        var cut = RenderList(new FakeTaskStore(_milk));
        Assert.IsFalse(cut.FindAll("button").Any(b => b.TextContent.Trim() == "Clear completed"));

        _auth.SetRoles("lead");
        cut.Render();

        Assert.IsNotNull(ButtonWithText(cut, "Clear completed"));
    }

    [TestMethod]
    public void ClearCompletedRemovesOnlyTheDoneTasks()
    {
        _auth.SetAuthorized("ana");
        _auth.SetRoles("lead");
        var done = new TaskItem(Guid.NewGuid(), "Pay rent", 1) { Done = true };
        var store = new FakeTaskStore(done, _dog);
        var cut = RenderList(store);

        ButtonWithText(cut, "Clear completed").Click();

        Assert.AreEqual(_dog, Assert.ContainsSingle(store.Items));
        Assert.AreEqual("Walk the dog", Assert.ContainsSingle(cut.FindAll("li span")).TextContent);
    }
}
