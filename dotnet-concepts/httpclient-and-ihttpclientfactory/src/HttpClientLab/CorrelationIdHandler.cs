namespace HttpClientLab;

public sealed class CorrelationIdHandler : DelegatingHandler
{
    public const string HeaderName = "X-Correlation-Id";

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // TODO-04: Add the X-Correlation-Id header (a new Guid in "N" format) unless the
        // request already carries one, then pass the request down the chain.
        return base.SendAsync(request, cancellationToken);
    }
}
