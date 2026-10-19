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

    [TestMethod]
    public async Task ShouldReturn401ToAnAnonymousCaller()
    {
        var response = await _api.Anonymous().GetAsync("/notes", Ct);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldLetAnyoneCallTheHealthEndpoint()
    {
        var response = await _api.Anonymous().GetAsync("/health", Ct);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturnTheOwnNoteToItsOwner()
    {
        var note = await CreateNoteAsAsync("alice", "buy milk");

        var response = await _api.ClientAs("alice").GetAsync($"/notes/{note.Id}", Ct);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(note, await response.Content.ReadFromJsonAsync<Note>(Ct));
    }

    [TestMethod]
    public async Task ShouldReturn403WhenAnotherUserReadsTheNote()
    {
        var note = await CreateNoteAsAsync("alice", "buy milk");

        var response = await _api.ClientAs("bob").GetAsync($"/notes/{note.Id}", Ct);

        // Bob is authenticated (not 401) and not allowed (403).
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturn403WhenAnotherUserDeletesTheNoteAndKeepIt()
    {
        var note = await CreateNoteAsAsync("alice", "buy milk");

        var response = await _api.ClientAs("bob").DeleteAsync($"/notes/{note.Id}", Ct);

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        var stillThere = await _api.ClientAs("alice").GetAsync($"/notes/{note.Id}", Ct);
        Assert.AreEqual(HttpStatusCode.OK, stillThere.StatusCode);
    }

    [TestMethod]
    public async Task ShouldLetAModeratorDeleteSomeoneElsesNote()
    {
        var note = await CreateNoteAsAsync("alice", "spam");

        var response = await _api.ClientAs("carol", Roles.Moderator).DeleteAsync($"/notes/{note.Id}", Ct);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        var gone = await _api.ClientAs("alice").GetAsync($"/notes/{note.Id}", Ct);
        Assert.AreEqual(HttpStatusCode.NotFound, gone.StatusCode);
    }

    [TestMethod]
    public async Task ShouldListOnlyTheCallersNotes()
    {
        await CreateNoteAsAsync("alice", "a1");
        await CreateNoteAsAsync("bob", "b1");

        var notes = await _api.ClientAs("alice").GetFromJsonAsync<List<Note>>("/notes", Ct);

        Assert.AreEqual("a1", Assert.ContainsSingle(notes!).Text);
    }

    [TestMethod]
    [DataRow(Roles.Moderator)]
    [DataRow(null)]
    public async Task ShouldReturn403OnAdminStatsToAnyoneWhoIsNotAnAdmin(string? role)
    {
        var client = role is null ? _api.ClientAs("bob") : _api.ClientAs("bob", role);

        var response = await client.GetAsync("/admin/stats", Ct);

        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturn401OnAdminStatsToAnAnonymousCaller()
    {
        var response = await _api.Anonymous().GetAsync("/admin/stats", Ct);

        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task ShouldReturnStatsToAnAdmin()
    {
        await CreateNoteAsAsync("alice", "a1");

        var response = await _api.ClientAs("root", Roles.Admin).GetAsync("/admin/stats", Ct);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var stats = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>(Ct);
        Assert.AreEqual(1, stats!["notes"]);
    }
}
