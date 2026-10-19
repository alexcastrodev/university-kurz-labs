using System.IO.Compression;

namespace CompressionLab;

public static class ZipTools
{
    // TODO-02
    public static byte[] CreateZip(IReadOnlyDictionary<string, byte[]> files)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, bytes) in files)
            {
                var entry = zip.CreateEntry(name, CompressionLevel.Optimal);
                using var s = entry.Open();
                s.Write(bytes);
            }
        }
        return ms.ToArray();
    }

    // TODO-03
    public static Dictionary<string, byte[]> ReadZip(byte[] zip, long maxTotalBytes)
    {
        var result = new Dictionary<string, byte[]>();
        long total = 0;
        using var ms = new MemoryStream(zip);
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);
        var buffer = new byte[8192];
        foreach (var entry in archive.Entries)
        {
            if (entry.FullName.EndsWith('/')) continue;
            using var s = entry.Open();
            using var content = new MemoryStream();
            int n;
            while ((n = s.Read(buffer)) > 0)
            {
                total += n; // count what really comes out, never trust entry.Length
                if (total > maxTotalBytes) throw new InvalidDataException("Archive expands beyond the allowed size.");
                content.Write(buffer, 0, n);
            }
            result[entry.FullName] = content.ToArray();
        }
        return result;
    }

    // TODO-04
    public static void ExtractSafely(byte[] zip, string destination)
    {
        Directory.CreateDirectory(destination);
        var prefix = Path.GetFullPath(destination) + Path.DirectorySeparatorChar;

        using var ms = new MemoryStream(zip);
        using var archive = new ZipArchive(ms, ZipArchiveMode.Read);

        // First pass: validate every name so nothing is written if one is bad.
        var targets = new List<(ZipArchiveEntry Entry, string Path)>();
        foreach (var entry in archive.Entries)
        {
            var full = Path.GetFullPath(Path.Combine(destination, entry.FullName));
            if (!full.StartsWith(prefix, StringComparison.Ordinal))
                throw new InvalidDataException($"Entry escapes the destination: {entry.FullName}");
            targets.Add((entry, full));
        }

        foreach (var (entry, path) in targets)
        {
            if (entry.FullName.EndsWith('/'))
            {
                Directory.CreateDirectory(path);
                continue;
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            entry.ExtractToFile(path, overwrite: true);
        }
    }
}
