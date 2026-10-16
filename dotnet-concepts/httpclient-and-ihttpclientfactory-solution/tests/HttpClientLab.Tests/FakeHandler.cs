using System.Net;
using System.Net.Http.Json;

namespace HttpClientLab.Tests;

public sealed class FakeHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> respond) : HttpMessageHandler
{
    public List<HttpRequestMessage> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        return respond(request, cancellationToken);
    }

    public static FakeHandler Json(object body) =>
        new((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(body) }));

    public static FakeHandler Status(HttpStatusCode status) =>
        new((_, _) => Task.FromResult(new HttpResponseMessage(status)));

    public static FakeHandler Hanging() =>
        new(async (_, ct) =>
        {
            await Task.Delay(Timeout.Infinite, ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
}
