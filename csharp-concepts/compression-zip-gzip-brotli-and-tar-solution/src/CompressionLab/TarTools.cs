using System.Formats.Tar;
using System.IO.Compression;

namespace CompressionLab;

public static class TarTools
{
    // TODO-05
    public static byte[] CreateTarGz(IReadOnlyDictionary<string, byte[]> files)
    {
        using var ms = new MemoryStream();
        using (var gzip = new GZipStream(ms, CompressionLevel.Optimal, leaveOpen: true))
        using (var tar = new TarWriter(gzip, TarEntryFormat.Pax, leaveOpen: true))
        {
            foreach (var (name, bytes) in files)
            {
                tar.WriteEntry(new PaxTarEntry(TarEntryType.RegularFile, name)
                {
                    DataStream = new MemoryStream(bytes)
                });
            }
        } // tar disposed first (end-of-archive blocks), then gzip (trailer)
        return ms.ToArray();
    }

    public static Dictionary<string, byte[]> ReadTarGz(byte[] tarGz)
    {
        var result = new Dictionary<string, byte[]>();
        using var ms = new MemoryStream(tarGz);
        using var gzip = new GZipStream(ms, CompressionMode.Decompress);
        using var tar = new TarReader(gzip);
        TarEntry? entry;
        while ((entry = tar.GetNextEntry()) is not null)
        {
            if (entry.DataStream is null) continue; // directories, links
            using var content = new MemoryStream();
            entry.DataStream.CopyTo(content);
            result[entry.Name] = content.ToArray();
        }
        return result;
    }
}
