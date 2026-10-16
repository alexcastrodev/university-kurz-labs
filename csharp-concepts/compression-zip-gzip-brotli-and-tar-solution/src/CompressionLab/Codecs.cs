using System.IO.Compression;

namespace CompressionLab;

public static class Codecs
{
    // TODO-00
    public static byte[] Gzip(byte[] data)
    {
        using var output = new MemoryStream();
        // leaveOpen keeps the MemoryStream usable; disposing the compressor writes the trailer.
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            gzip.Write(data);
        }
        return output.ToArray();
    }

    public static byte[] Gunzip(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        gzip.CopyTo(output);
        return output.ToArray();
    }

    // TODO-01
    public static byte[] BrotliCompress(byte[] data, CompressionLevel level)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, level, leaveOpen: true))
        {
            brotli.Write(data);
        }
        return output.ToArray();
    }

    public static byte[] BrotliDecompress(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return output.ToArray();
    }
}
