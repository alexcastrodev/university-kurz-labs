namespace Freight;

// Stamps a fresh Idempotency-Key on every POST that passes through it, so the API can recognize a
// repeated request. Where it sits in the handler chain decides what "every POST" means: outside the
// resilience handler it runs once per call, inside it would run once per attempt.
public sealed class IdempotencyKeyHandler : DelegatingHandler
{
    public const string HeaderName = "Idempotency-Key";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method == HttpMethod.Post)
        {
            request.Headers.Remove(HeaderName);
            request.Headers.Add(HeaderName, Guid.NewGuid().ToString("N"));
        }

        return base.SendAsync(request, cancellationToken);
    }
}
