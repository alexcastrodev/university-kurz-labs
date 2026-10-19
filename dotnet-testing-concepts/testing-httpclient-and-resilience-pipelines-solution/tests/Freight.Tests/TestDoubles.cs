using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Freight.Tests;

// What one attempt really sent, copied at the moment it was sent.
internal sealed record SentRequest(HttpMethod Method, string Path, string Body, string? IdempotencyKey);

// A hand-written fake API: no mocking library, no server. Each attempt takes the next scripted answer.
// Two rules make it honest:
// - An unscripted request fails the test. A fake that answers 200 to everything lets a retry test pass
//   with zero retries.
// - It snapshots each attempt. A retry sends the SAME HttpRequestMessage instance again, so a list of
//   messages read after the call shows the last attempt's headers three times. And it reads the body
//   with CopyToAsync, like a transport does: ReadAsStringAsync would buffer the content and hide a body
//   that cannot be sent twice.
internal sealed class FakeFreightApi : HttpMessageHandler
{
    private readonly ConcurrentQueue<Func<CancellationToken, Task<HttpResponseMessage>>> _script = new();
    private readonly ConcurrentQueue<SentRequest> _sent = new();

    public IReadOnlyList<SentRequest> Sent => _sent.ToArray();

    public FakeFreightApi Respond(HttpStatusCode status, string? json = null)
    {
        _script.Enqueue(_ => Task.FromResult(new HttpResponseMessage(status)
        {
            Content = json is null ? null : new StringContent(json, Encoding.UTF8, "application/json"),
        }));
        return this;
    }

    // An attempt that never answers. It honors the token, like a real connection the client gives up on.
    public FakeFreightApi Hang()
    {
        _script.Enqueue(async cancellationToken =>
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK); // never reached
        });
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = "";
        if (request.Content is not null)
        {
            using var copy = new MemoryStream();
            await request.Content.CopyToAsync(copy, cancellationToken);
            body = Encoding.UTF8.GetString(copy.ToArray());
        }

        request.Headers.TryGetValues("Idempotency-Key", out var keys);
        _sent.Enqueue(new SentRequest(request.Method, request.RequestUri!.AbsolutePath, body, keys?.Single()));

        if (!_script.TryDequeue(out var answer))
        {
            Assert.Fail($"Unscripted request: {request.Method} {request.RequestUri}");
        }

        return await answer(cancellationToken);
    }
}

// A stream that can be read once, like an incoming request body or a network download.
internal sealed class ForwardOnlyStream(string text) : Stream
{
    private readonly MemoryStream _inner = new(Encoding.UTF8.GetBytes(text));

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
}
