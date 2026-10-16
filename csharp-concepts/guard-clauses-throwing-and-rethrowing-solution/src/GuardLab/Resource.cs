namespace GuardLab;

public sealed class Resource : IDisposable
{
    private bool _disposed;

    public string Read()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return "data";
    }

    public void Dispose() => _disposed = true;
}
