using Notes.Api.Notes;

namespace Notes.Api.Tests;

// No host, no HTTP: the handler and the policies are plain classes.
[TestClass]
public sealed class NotesAuthorizationTests
{
    private static readonly Note AlicesNote = new(1, "alice", "buy milk");

    // TODO-05: Call NoteOwnerHandler directly. Build a ClaimsPrincipal (remember the authenticationType), an
    // AuthorizationHandlerContext([requirement], user, note), await handler.HandleAsync(context) and read
    // context.HasSucceeded.
    [TestMethod]
    public async Task TheHandlerShouldAllowTheOwner()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task TheHandlerShouldRefuseAnotherUser()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task TheHandlerShouldAllowAModerator()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    [TestMethod]
    public async Task TheHandlerShouldNotTreatAnAdminAsAModerator()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-05: write this test.");
    }

    // TODO-06: The real registration, without HTTP. new ServiceCollection().AddLogging().AddNotesAuthorization()
    // gives you an IAuthorizationService wired exactly like Program.cs. Use its AuthorizeAsync overloads with
    // the policy names in Policies.
    [TestMethod]
    [DataRow("alice", null, true)]
    [DataRow("bob", null, false)]
    [DataRow("carol", Roles.Moderator, true)]
    public async Task TheNoteOwnerPolicyShouldDecideOnTheNote(string userId, string? role, bool expected)
    {
        _ = AlicesNote;
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }

    [TestMethod]
    public async Task TheAdminPolicyShouldRequireTheAdminRole()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-06: write this test.");
    }
}
