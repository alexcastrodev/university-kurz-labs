using System.Buffers;

namespace GcLab;

public static class Buffers
{
    // Provided: every call allocates a new array (a new LOH object for big sizes).
    public static int UseNewArray(int size, Func<byte[], int> work) => work(new byte[size]);

    public static int UsePooledArray(int size, Func<byte[], int> work)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            return work(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
