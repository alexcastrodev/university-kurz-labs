using AngleSharp.Dom;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace TaskBoard.Tests;

// Read src/TaskBoard/TaskList.razor first: it injects ITaskStore and IJSRuntime, loads its tasks in
// OnInitializedAsync, takes a cascading BoardSettings, and contains an <AuthorizeView>.
[TestClass]
public sealed class TaskListTests : BunitContext
{
    private readonly TaskItem _milk = new(Guid.NewGuid(), "Buy milk", 3);
    private readonly TaskItem _dog = new(Guid.NewGuid(), "Walk the dog", 2);

    // TODO-02: Register the store in Services as the ITaskStore, then render TaskList, wrapped in the cascading
    // settings when they are given (p => p.AddCascadingValue(settings)). Run one test and read the exception:
    // something else has to be set up before TaskList can render. Do it once for every test in this class.
    private IRenderedComponent<TaskList> RenderList(FakeTaskStore store, BoardSettings? settings = null) =>
        throw new NotImplementedException("TODO-02: register the store and render TaskList.");

    // TODO-03: Loading is asynchronous. FakeTaskStore { HoldLoading = true } keeps GetAllAsync waiting until you
    // call store.Release(). After Release, the component renders again a moment later, not immediately.
    [TestMethod]
    public void ShowsLoadingUntilTheStoreAnswers()
    {
        // Check the loading text, release the store, then wait (WaitForAssertion) for the two <li>.
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public void AnEmptyStoreShowsTheEmptyMessage()
    {
        // Careful: "there is no <li>" is already true while the list is still loading.
        Assert.Fail("TODO-03: write this test.");
    }

    // TODO-04: Interactions. bUnit's JSInterop is strict: a JS call the test did not set up throws.
    [TestMethod]
    public void TickingTheCheckboxMarksTheTaskDoneAndSavesIt()
    {
        // Find the checkbox by its aria-label and Change(true) it. Check the task, store.Saves and the checked attribute.
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public void ConfirmingTheDeleteRemovesTheTask()
    {
        // The component asks window.confirm first: JSInterop.Setup<bool>("confirm", "<the exact message>").SetResult(...)
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public void DecliningTheDeleteKeepsTheTask()
    {
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public void AddingATaskSavesItShowsItAndAnnouncesIt()
    {
        // The form lives inside TaskList. Type a title with spaces around it. The component then calls
        // InvokeVoidAsync("taskBoard.announce", ...): set it up with SetupVoid and check it with VerifyInvoke.
        Assert.Fail("TODO-04: write this test.");
    }

    // TODO-05: Cascading values and authorization.
    [TestMethod]
    public void HideCompletedFromTheCascadingSettingsHidesDoneTasks()
    {
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public void OnlyALeadSeesClearCompleted()
    {
        // AddAuthorization() returns a BunitAuthorizationContext: SetAuthorized("bob"), then SetRoles("lead").
        // Find the button by its text, not by its CSS class.
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public void ClearCompletedRemovesOnlyTheDoneTasks()
    {
        Assert.Fail("TODO-05: write this test.");
    }
}
