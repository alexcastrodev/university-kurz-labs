using System.IO.Compression;

namespace CompressionLab;

public static class Codecs
{
    // TODO-00: compress with GZipStream into a MemoryStream and return the bytes.
    // Trap: the compressor only writes the header/trailer when disposed, and
    // disposing it also disposes the MemoryStream unless leaveOpen is true.
    public static byte[] Gzip(byte[] data) => throw new NotImplementedException();

    public static byte[] Gunzip(byte[] compressed) => throw new NotImplementedException();

    // TODO-01: same idea with BrotliStream, honoring the requested CompressionLevel.
    public static byte[] BrotliCompress(byte[] data, CompressionLevel level) => throw new NotImplementedException();

    public static byte[] BrotliDecompress(byte[] compressed) => throw new NotImplementedException();
}
