namespace PartialLab;

// Defining declarations only. The implementing declarations are in
// Widget.Members.cs (property, indexer, method) and Widget.Lifecycle.cs (constructor, event).
public partial class Widget
{
    public partial Widget(int size);

    public partial string Title { get; set; }

    public partial int this[int i] { get; }

    public partial event Action<string> Changed;

    public partial string Describe();

    public void Rename(string title)
    {
        Title = title;
        RaiseChanged(Title);
    }
}
