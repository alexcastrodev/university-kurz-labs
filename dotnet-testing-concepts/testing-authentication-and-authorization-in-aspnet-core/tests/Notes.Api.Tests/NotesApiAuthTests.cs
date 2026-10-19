using System.Net;
using System.Net.Http.Json;
using Notes.Api.Notes;

namespace Notes.Api.Tests;

// The HTTP contract of authentication and authorization, with the fake scheme. One host per test:
// the NoteStore is a singleton and the tests run in parallel.
[TestClass]
[TestCategory("Integration")]
public sealed class NotesApiAuthTests(TestContext testContext) : IAsyncDisposable
{
    private readonly NotesApiFactory _api = new();

    private CancellationToken Ct => testContext.CancellationToken;

    public ValueTask DisposeAsync() => _api.DisposeAsync();

    private async Task<Note> CreateNoteAsAsync(string userId, string text)
    {
        var response = await _api.ClientAs(userId).PostAsJsonAsync("/notes", new CreateNoteRequest(text), Ct);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<Note>(Ct))!;
    }

    // TODO-02: 401 means "who are you?", 403 means "I know who you are, and no". Assert the exact status:
    // Assert.IsFalse(response.IsSuccessStatusCode) passes for both, so it cannot tell them apart.
    [TestMethod]
    public async Task ShouldReturn401ToAnAnonymousCaller()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-02: write this test.");
    }

    [TestMethod]
    public async Task ShouldLetAnyoneCallTheHealthEndpoint()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-02: write this test.");
    }

    // TODO-03: Resource-based authorization through HTTP. Alice owns the note; Bob is another user; Carol is a
    // moderator. A suite where every request comes from the owner proves nothing about the others.
    [TestMethod]
    public async Task ShouldReturnTheOwnNoteToItsOwner()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturn403WhenAnotherUserReadsTheNote()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturn403WhenAnotherUserDeletesTheNoteAndKeepIt()
    {
        // The status is half of it: is the note still there afterwards?
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldLetAModeratorDeleteSomeoneElsesNote()
    {
        // 204, and then the owner gets a 404.
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    [TestMethod]
    public async Task ShouldListOnlyTheCallersNotes()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-03: write this test.");
    }

    // TODO-04: The Admin policy. Testing only as an admin shows that admins get in, not that others stay out.
    [TestMethod]
    [DataRow(Roles.Moderator)]
    [DataRow(null)]
    public async Task ShouldReturn403OnAdminStatsToAnyoneWhoIsNotAnAdmin(string? role)
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturn401OnAdminStatsToAnAnonymousCaller()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }

    [TestMethod]
    public async Task ShouldReturnStatsToAnAdmin()
    {
        await Task.CompletedTask;
        Assert.Fail("TODO-04: write this test.");
    }
}
