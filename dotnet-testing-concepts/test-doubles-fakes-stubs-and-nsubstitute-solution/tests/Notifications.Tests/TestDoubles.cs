using System.Net;

namespace Notifications.Tests;

// A hand-written fake: a working in-memory implementation of the port. Tests read its state after
// the act step instead of asking which methods were called.
internal sealed class InMemoryNotificationLog : INotificationLog
{
    private readonly HashSet<string> _keys = [];

    public IReadOnlyCollection<string> Keys => _keys;

    public Task<bool> ContainsAsync(string key, CancellationToken ct) => Task.FromResult(_keys.Contains(key));

    public Task AddAsync(string key, CancellationToken ct)
    {
        _keys.Add(key);
        return Task.CompletedTask;
    }
}

// A hand-written stub for HttpClient. HttpMessageHandler.SendAsync is protected, so a small subclass
// is simpler than any mocking library. It answers with a fixed status and keeps the last request.
internal sealed class StubHttpHandler(HttpStatusCode status) : HttpMessageHandler
{
    public HttpRequestMessage? LastRequest { get; private set; }

    public string? LastBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        LastRequest = request;
        LastBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(ct);
        return new HttpResponseMessage(status);
    }
}
