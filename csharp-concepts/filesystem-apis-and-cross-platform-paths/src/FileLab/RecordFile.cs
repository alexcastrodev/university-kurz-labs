using Microsoft.Win32.SafeHandles;

namespace FileLab;

/// <summary>A file of fixed-size 4-byte int records, accessed by index without a stream position.</summary>
public sealed class RecordFile : IDisposable
{
    public const int RecordSize = sizeof(int);

    private readonly SafeFileHandle _handle;

    public RecordFile(string path)
    {
        // TODO-05 (part 1): Open (or create) the file for reading and writing with File.OpenHandle
        //   and keep the SafeFileHandle in _handle. There is no FileStream here on purpose.
        throw new NotImplementedException("Not implemented yet.");
    }

    public int Count
    {
        get
        {
            // TODO-05 (part 2): Number of whole records in the file, from RandomAccess.GetLength.
            throw new NotImplementedException("Not implemented yet.");
        }
    }

    public int Read(int index)
    {
        // TODO-05 (part 3): Read the record at the given index with RandomAccess.Read and an explicit
        //   offset (index * RecordSize). Throw ArgumentOutOfRangeException when index is negative or >= Count.
        throw new NotImplementedException("Not implemented yet.");
    }

    public void Write(int index, int value)
    {
        // TODO-05 (part 4): Write the record at index with RandomAccess.Write. Writing past the end
        //   grows the file; the gap reads as zeros. Throw ArgumentOutOfRangeException for a negative index.
        //   It must be safe to call from several threads on different indexes.
        throw new NotImplementedException("Not implemented yet.");
    }

    public void Dispose() => _handle.Dispose();
}
