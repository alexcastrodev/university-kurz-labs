namespace BlazorComponentLifecycleAndParameters;

/// <summary>Collects the names of the lifecycle hooks a component runs. Provided code, no TODO here.</summary>
public sealed class CallLog
{
    private readonly List<string> entries = [];

    public IReadOnlyList<string> Entries => entries;

    public void Add(string entry) => entries.Add(entry);

    public void Clear() => entries.Clear();
}

/// <summary>Something slow to load. Tests control completion with a TaskCompletionSource.</summary>
public interface IDataSource
{
    Task<string> LoadAsync();
}

public interface IProductService
{
    Task<string> GetNameAsync(int id);
}

/// <summary>A plain event source, like a timer or a message bus, that raises on any thread.</summary>
public sealed class TickSource
{
    public event Action? Ticked;

    public int SubscriberCount => Ticked?.GetInvocationList().Length ?? 0;

    public void Raise() => Ticked?.Invoke();
}
