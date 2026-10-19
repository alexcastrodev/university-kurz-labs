using System.Net.Http.Json;

namespace Notifications;

// The production adapter behind ISmsSender. It is sealed and talks HTTP, so the service tests never
// use it: they replace the ISmsSender port instead.
public sealed class HttpSmsSender(HttpClient http) : ISmsSender
{
    public async Task<bool> SendAsync(string phoneNumber, string text, CancellationToken ct)
    {
        using var response = await http.PostAsJsonAsync("messages", new SmsRequest(phoneNumber, text), ct);
        return response.IsSuccessStatusCode;
    }

    public sealed record SmsRequest(string To, string Text);
}
