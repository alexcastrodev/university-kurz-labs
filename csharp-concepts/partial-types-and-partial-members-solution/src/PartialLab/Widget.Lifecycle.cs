namespace PartialLab;

public partial class Widget
{
    private int _size;
    private Action<string>? _changed;

    public partial Widget(int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        _size = size;
    }

    public partial event Action<string> Changed
    {
        add => _changed += value;
        remove => _changed -= value;
    }

    private void RaiseChanged(string title) => _changed?.Invoke(title);
}
