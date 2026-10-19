using System.IO.Compression;

namespace CompressionLab;

public static class ZipTools
{
    // TODO-02: build a ZIP in memory (ZipArchiveMode.Create), one entry per file.
    // Remember leaveOpen and that the archive must be disposed before ToArray().
    public static byte[] CreateZip(IReadOnlyDictionary<string, byte[]> files) => throw new NotImplementedException();

    // TODO-03: read every file entry (skip names ending in '/') into a dictionary keyed by FullName.
    // Count the bytes while copying and throw InvalidDataException when the total exceeds
    // maxTotalBytes. Do not trust entry.Length, headers can lie.
    public static Dictionary<string, byte[]> ReadZip(byte[] zip, long maxTotalBytes) => throw new NotImplementedException();

    // TODO-04: extract into destination (create it if missing) without ever writing outside it.
    // Resolve each entry with Path.GetFullPath and compare against the destination full path
    // WITH a trailing separator (so "out-evil" does not pass for "out").
    // Throw InvalidDataException for a bad entry, and validate ALL entries before writing any.
    public static void ExtractSafely(byte[] zip, string destination) => throw new NotImplementedException();
}
