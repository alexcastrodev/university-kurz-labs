using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Notes.Api.Notes;

namespace Notes.Api.Tests;

// No host, no HTTP: the handler and the policies are plain classes.
[TestClass]
public sealed class NotesAuthorizationTests
{
    private static readonly Note AlicesNote = new(1, "alice", "buy milk");

    private static ClaimsPrincipal User(string id, params string[] roles) =>
        new(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, id), .. roles.Select(r => new Claim(ClaimTypes.Role, r))],
            authenticationType: "unit-test"));

    private static async Task<bool> HandlerAllowsAsync(ClaimsPrincipal user, Note note)
    {
        var requirement = new NoteOwnerRequirement();
        var context = new AuthorizationHandlerContext([requirement], user, note);

        await new NoteOwnerHandler().HandleAsync(context);

        return context.HasSucceeded;
    }

    [TestMethod]
    public async Task TheHandlerShouldAllowTheOwner() =>
        Assert.IsTrue(await HandlerAllowsAsync(User("alice"), AlicesNote));

    [TestMethod]
    public async Task TheHandlerShouldRefuseAnotherUser() =>
        Assert.IsFalse(await HandlerAllowsAsync(User("bob"), AlicesNote));

    [TestMethod]
    public async Task TheHandlerShouldAllowAModerator() =>
        Assert.IsTrue(await HandlerAllowsAsync(User("carol", Roles.Moderator), AlicesNote));

    [TestMethod]
    public async Task TheHandlerShouldNotTreatAnAdminAsAModerator() =>
        Assert.IsFalse(await HandlerAllowsAsync(User("root", Roles.Admin), AlicesNote));

    // The real registration from Program.cs, resolved from a container: policy names and handler wiring.
    private static IAuthorizationService AuthorizationService() =>
        new ServiceCollection()
            .AddLogging()
            .AddNotesAuthorization()
            .BuildServiceProvider()
            .GetRequiredService<IAuthorizationService>();

    [TestMethod]
    [DataRow("alice", null, true)]
    [DataRow("bob", null, false)]
    [DataRow("carol", Roles.Moderator, true)]
    public async Task TheNoteOwnerPolicyShouldDecideOnTheNote(string userId, string? role, bool expected)
    {
        var user = role is null ? User(userId) : User(userId, role);

        var result = await AuthorizationService().AuthorizeAsync(user, AlicesNote, Policies.NoteOwner);

        Assert.AreEqual(expected, result.Succeeded);
    }

    [TestMethod]
    public async Task TheAdminPolicyShouldRequireTheAdminRole()
    {
        var service = AuthorizationService();

        Assert.IsTrue((await service.AuthorizeAsync(User("root", Roles.Admin), Policies.Admin)).Succeeded);
        Assert.IsFalse((await service.AuthorizeAsync(User("carol", Roles.Moderator), Policies.Admin)).Succeeded);
    }
}
