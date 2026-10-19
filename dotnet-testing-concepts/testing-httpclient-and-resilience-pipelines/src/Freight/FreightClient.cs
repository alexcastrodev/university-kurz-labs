using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Freight;

// Typed client for a freight API. The resilience pipeline (AddFreightClient) retries and times out;
// this class turns what is left into a result the caller can act on.
//
// The rules the tests must protect:
// 1. A 4xx (other than 408 and 429) is Rejected and is not retried.
// 2. A 5xx, 408 or 429 is retried; when the retries run out the result is Unavailable.
// 3. Every attempt of one call carries the same Idempotency-Key and the same body.
// 4. A slow attempt is cut by the attempt timeout and reported as Unavailable "timeout".
// 5. A cancellation by the caller is not a timeout: OperationCanceledException reaches the caller.
// 6. After repeated failures the circuit opens and calls stop reaching the API.
// 7. A 200 whose body is not a quote is a contract bug, not an outage: JsonException reaches the caller.
// 8. A manifest read from a forward-only stream is sent in full on every attempt.
public sealed class FreightClient(HttpClient http)
{
    public Task<FreightResult> GetQuoteAsync(QuoteRequest request, CancellationToken cancellationToken) =>
        SendAsync(async () =>
        {
            using var response = await http.PostAsJsonAsync("quotes", request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return Failure(response.StatusCode);
            }

            var quote = await response.Content.ReadFromJsonAsync<Quote>(cancellationToken);
            return FreightResult.Ok(quote);
        });

    public Task<FreightResult> UploadManifestAsync(string shipmentId, Stream manifest, CancellationToken cancellationToken) =>
        SendAsync(async () =>
        {
            using var content = new StreamContent(manifest);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            // The stream may be forward-only (an incoming request body, a download). Buffer it, or a
            // retry finds it already consumed.
            await content.LoadIntoBufferAsync(cancellationToken);

            using var response = await http.PutAsync($"shipments/{shipmentId}/manifest", content, cancellationToken);
            return response.IsSuccessStatusCode ? FreightResult.Ok() : Failure(response.StatusCode);
        });

    private static async Task<FreightResult> SendAsync(Func<Task<FreightResult>> send)
    {
        try
        {
            return await send();
        }
        catch (TimeoutRejectedException)
        {
            return FreightResult.Unavailable("timeout");
        }
        catch (BrokenCircuitException)
        {
            return FreightResult.Unavailable("circuit open");
        }
        catch (HttpRequestException exception)
        {
            return FreightResult.Unavailable($"network: {exception.Message}");
        }
    }

    private static FreightResult Failure(HttpStatusCode status) =>
        (int)status >= 500 || status is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
            ? FreightResult.Unavailable($"status {(int)status}")
            : FreightResult.Rejected($"status {(int)status}");
}
