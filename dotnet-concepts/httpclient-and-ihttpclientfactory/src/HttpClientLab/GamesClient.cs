using System.Net.Http.Json;

namespace HttpClientLab;

public sealed class GamesClient(HttpClient http)
{
    public async Task<Game?> GetGameAsync(int id, CancellationToken cancellationToken = default)
    {
        // TODO-05: GET /games/{id}. Return null for 404, throw for any other failure
        // status, and read the JSON body into a Game otherwise. Dispose the response.

        // TODO-06: A timeout from HttpClient.Timeout is an OperationCanceledException that
        // the caller did not ask for. Turn it into GameServiceTimeoutException, but let a
        // cancellation requested through cancellationToken keep propagating unchanged.
        await Task.CompletedTask;
        throw new NotImplementedException("Not implemented yet.");
    }

    public async Task<string?> ReadFirstLineAsync(string path, CancellationToken cancellationToken = default)
    {
        // TODO-07: GET the path without buffering the body (HttpCompletionOption), check the
        // status, and return only the first line of the response stream.
        await Task.CompletedTask;
        throw new NotImplementedException("Not implemented yet.");
    }

    public IAsyncEnumerable<Game?> StreamGamesAsync(CancellationToken cancellationToken = default)
    {
        // TODO-08 (optional): GET /games and yield each Game as it is deserialized
        // (GetFromJsonAsAsyncEnumerable), instead of waiting for the whole list.
        throw new NotImplementedException("Optional.");
    }

    public Task<Game?> GetGameAsync(int id, TimeSpan perCallTimeout, CancellationToken cancellationToken = default)
    {
        // TODO-09 (optional): Apply perCallTimeout to this call only, with a linked
        // CancellationTokenSource and CancelAfter, and report it as GameServiceTimeoutException.
        throw new NotImplementedException("Optional.");
    }
}
