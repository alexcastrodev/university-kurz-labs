using System.Net;

namespace Notifications.Tests;

// TODO-00: Write a hand-written fake of INotificationLog: a working in-memory implementation, a few
// lines long. Keep the keys in a HashSet<string> and expose them through Keys, so a test can read what
// was recorded after the act step. No mocking library here.
internal sealed class InMemoryNotificationLog : INotificationLog
{
    public IReadOnlyCollection<string> Keys => throw new NotImplementedException("TODO-00: expose the recorded keys.");

    public Task<bool> ContainsAsync(string key, CancellationToken ct) =>
        throw new NotImplementedException("TODO-00: answer whether the key was recorded.");

    public Task AddAsync(string key, CancellationToken ct) =>
        throw new NotImplementedException("TODO-00: record the key.");
}

// TODO-08 (optional): Make this stub answer every request with the status it was given, and remember
// the last request and its body (read the body inside SendAsync, before the request is disposed).
// HttpMessageHandler.SendAsync is protected, so a small subclass is simpler than a mocking library.
internal sealed class StubHttpHandler(HttpStatusCode status) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    public string? LastBody { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
        throw new NotImplementedException($"TODO-08 (optional): answer with {status} and keep the request.");
}
