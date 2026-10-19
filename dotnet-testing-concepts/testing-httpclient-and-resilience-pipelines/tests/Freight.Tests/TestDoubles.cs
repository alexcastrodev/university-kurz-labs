using System.Net;
using System.Text;

namespace Freight.Tests;

// What one attempt really sent, copied at the moment it was sent.
internal sealed record SentRequest(HttpMethod Method, string Path, string Body, string? IdempotencyKey);

// TODO-00: Write a hand-written fake API: an HttpMessageHandler, no mocking library, no server.
//   - Respond(status, json?) and Hang() queue the answer for the next attempt. Hang never answers, but it
//     honors the cancellation token (Task.Delay(Timeout.Infinite, token)), like a real connection.
//   - SendAsync records a SentRequest for every attempt, THEN takes the next scripted answer.
//   - An unscripted request fails the test. A fake that answers 200 to everything lets a retry test pass
//     with zero retries.
//   - Snapshot each attempt at send time. A retry sends the SAME HttpRequestMessage instance again, so a
//     list of messages read after the call shows the last attempt's headers three times.
//   - Read the body with CopyToAsync into a MemoryStream, like a transport does. ReadAsStringAsync buffers
//     the content and hides a body that cannot be sent twice. Not reading it at all hides it too.
internal sealed class FakeFreightApi : HttpMessageHandler
{
    public IReadOnlyList<SentRequest> Sent => throw new NotImplementedException("TODO-00: what each attempt sent.");

    public FakeFreightApi Respond(HttpStatusCode status, string? json = null) =>
        throw new NotImplementedException("TODO-00: queue a response.");

    public FakeFreightApi Hang() =>
        throw new NotImplementedException("TODO-00: queue an attempt that never answers.");

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        throw new NotImplementedException("TODO-00: snapshot the attempt, then answer from the script.");
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
