using System.Net;

namespace ResilienceLab.Tests;

public sealed class ScriptedHandler(Func<int, CancellationToken, Task<HttpResponseMessage>> script) : HttpMessageHandler
{
    private int _hits;

    public int Hits => _hits;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        script(Interlocked.Increment(ref _hits), cancellationToken);

    public static ScriptedHandler FailTimesThenOk(int failures, HttpStatusCode failure = HttpStatusCode.ServiceUnavailable) =>
        new((attempt, _) => Task.FromResult(new HttpResponseMessage(attempt <= failures ? failure : HttpStatusCode.OK)));

    public static ScriptedHandler Always(HttpStatusCode status) =>
        new((_, _) => Task.FromResult(new HttpResponseMessage(status)));

    public static ScriptedHandler Hanging() =>
        new(async (_, ct) =>
        {
            await Task.Delay(Timeout.Infinite, ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
}
