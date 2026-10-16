namespace PartialLab;

public partial class Widget
{
    private int _size;
    private Action<string>? _changed;

    public partial Widget(int size)
    {
        // TODO-03: Throw ArgumentOutOfRangeException when size is zero or negative,
        //   otherwise store it in _size.
        throw new NotImplementedException("Not implemented yet.");
    }

    public partial event Action<string> Changed
    {
        // TODO-03: Implement add and remove over the _changed delegate.
        add => throw new NotImplementedException("Not implemented yet.");
        remove => throw new NotImplementedException("Not implemented yet.");
    }

    private void RaiseChanged(string title)
    {
        // TODO-03: Invoke the event handlers with the title, if there are any.
        throw new NotImplementedException("Not implemented yet.");
    }
}
