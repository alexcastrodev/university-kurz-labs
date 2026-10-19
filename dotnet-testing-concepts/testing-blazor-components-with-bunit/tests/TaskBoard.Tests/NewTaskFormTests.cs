namespace TaskBoard.Tests;

// TODO-01: The EditForm in src/TaskBoard/NewTaskForm.razor. Fill the inputs with Change(...) and send the form
// with Find("form").Submit(). Select the inputs by their id (#title, #priority), not by a CSS class.
[TestClass]
public sealed class NewTaskFormTests : BunitContext
{
    [TestMethod]
    public void SubmittingWithoutATitleShowsTheMessageAndCreatesNothing()
    {
        // Two checks: the validation message is in the markup, and OnCreate was never raised.
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void APriorityOutOfRangeIsRejected()
    {
        Assert.Fail("TODO-01: write this test.");
    }

    [TestMethod]
    public void AValidSubmitRaisesOnCreateAndClearsTheForm()
    {
        // Check the title and priority OnCreate received, and the value attribute of #title afterwards.
        Assert.Fail("TODO-01: write this test.");
    }
}
