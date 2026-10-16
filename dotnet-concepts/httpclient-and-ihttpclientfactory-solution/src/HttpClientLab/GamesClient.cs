using System.Net;
using System.Net.Http.Json;

namespace HttpClientLab;

public sealed class GamesClient(HttpClient http)
{
    public async Task<Game?> GetGameAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await http.GetAsync($"/games/{id}", cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Game>(cancellationToken);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new GameServiceTimeoutException(ex);
        }
    }

    public async Task<string?> ReadFirstLineAsync(string path, CancellationToken cancellationToken = default)
    {
        using var response = await http.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        return await reader.ReadLineAsync(cancellationToken);
    }

    public IAsyncEnumerable<Game?> StreamGamesAsync(CancellationToken cancellationToken = default) =>
        http.GetFromJsonAsAsyncEnumerable<Game>("/games", cancellationToken);

    public async Task<Game?> GetGameAsync(int id, TimeSpan perCallTimeout, CancellationToken cancellationToken = default)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linked.CancelAfter(perCallTimeout);
        try
        {
            return await GetGameAsync(id, linked.Token);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new GameServiceTimeoutException(ex);
        }
    }
}
