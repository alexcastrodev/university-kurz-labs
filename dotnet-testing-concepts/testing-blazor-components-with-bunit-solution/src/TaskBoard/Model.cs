using System.ComponentModel.DataAnnotations;

namespace TaskBoard;

public sealed class TaskItem(Guid id, string title, int priority)
{
    public Guid Id { get; } = id;
    public string Title { get; } = title;
    public int Priority { get; } = priority;
    public bool Done { get; set; }
}

// What the form edits. The attributes are the validation rules EditForm shows.
public sealed class NewTask
{
    [Required(ErrorMessage = "Give the task a title.")]
    [StringLength(40, ErrorMessage = "Keep the title under 40 characters.")]
    public string Title { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Priority goes from 1 to 5.")]
    public int Priority { get; set; } = 3;
}

// Passed down the tree with <CascadingValue>.
public sealed record BoardSettings(bool HideCompleted)
{
    public static readonly BoardSettings Default = new(HideCompleted: false);
}

public interface ITaskStore
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync();
    Task SaveAsync(TaskItem task);
    Task DeleteAsync(Guid id);
}
