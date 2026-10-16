using Microsoft.Win32.SafeHandles;

namespace FileLab;

/// <summary>A file of fixed-size 4-byte int records, accessed by index without a stream position.</summary>
public sealed class RecordFile : IDisposable
{
    public const int RecordSize = sizeof(int);

    private readonly SafeFileHandle _handle;

    public RecordFile(string path)
    {
        _handle = File.OpenHandle(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }

    public int Count => (int)(RandomAccess.GetLength(_handle) / RecordSize);

    public int Read(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));
        Span<byte> buffer = stackalloc byte[RecordSize];
        RandomAccess.Read(_handle, buffer, (long)index * RecordSize);
        return BitConverter.ToInt32(buffer);
    }

    public void Write(int index, int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        Span<byte> buffer = stackalloc byte[RecordSize];
        BitConverter.TryWriteBytes(buffer, value);
        RandomAccess.Write(_handle, buffer, (long)index * RecordSize);
    }

    public void Dispose() => _handle.Dispose();
}
