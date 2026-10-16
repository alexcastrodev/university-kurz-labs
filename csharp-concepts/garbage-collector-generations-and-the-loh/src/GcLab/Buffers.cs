using System.Buffers;

namespace GcLab;

public static class Buffers
{
    // Provided: every call allocates a new array (a new LOH object for big sizes).
    public static int UseNewArray(int size, Func<byte[], int> work) => work(new byte[size]);

    public static int UsePooledArray(int size, Func<byte[], int> work)
    {
        // TODO-05: Rent a buffer of at least `size` bytes from ArrayPool<byte>.Shared, pass it
        //   to `work`, and return the result. Give the buffer back to the pool in a finally
        //   block, even when `work` throws. Remember the buffer may be longer than `size`.
        throw new NotImplementedException("Not implemented yet.");
    }
}
