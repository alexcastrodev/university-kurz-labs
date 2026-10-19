namespace TaskBoard.Tests;

// An in-memory ITaskStore. HoldLoading makes GetAllAsync wait until the test calls Release(),
// so a test can look at the component while it is still loading.
internal sealed class FakeTaskStore(params TaskItem[] seed) : ITaskStore
{
    private readonly TaskCompletionSource _gate = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public List<TaskItem> Items { get; } = [.. seed];

    public int Saves { get; private set; }

    public bool HoldLoading { get; init; }

    public void Release() => _gate.SetResult();

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync()
    {
        if (HoldLoading)
        {
            await _gate.Task;
        }

        return Items.ToList();
    }

    public Task SaveAsync(TaskItem task)
    {
        Saves++;
        if (!Items.Contains(task))
        {
            Items.Add(task);
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        Items.RemoveAll(t => t.Id == id);
        return Task.CompletedTask;
    }
}
