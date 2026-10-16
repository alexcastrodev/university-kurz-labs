using System.Formats.Tar;
using System.IO.Compression;

namespace CompressionLab;

public static class TarTools
{
    // TODO-05: write a .tar.gz in memory: a TarWriter (Pax format) on top of a GZipStream.
    // Tar has no compression of its own. Dispose order matters: TarWriter first, then GZipStream.
    public static byte[] CreateTarGz(IReadOnlyDictionary<string, byte[]> files) => throw new NotImplementedException();

    // Read it back: GZipStream (decompress) under a TarReader; collect entries that have a DataStream.
    public static Dictionary<string, byte[]> ReadTarGz(byte[] tarGz) => throw new NotImplementedException();
}
