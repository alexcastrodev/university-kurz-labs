using System.IO.Compression;
using System.Text;

namespace CompressionLab.Tests;

public class CompressionTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
    }

    private static byte[] Repetitive(int lines = 2000) =>
        Encoding.UTF8.GetBytes(string.Concat(Enumerable.Repeat("{\"event\":\"login\",\"ok\":true}\n", lines)));

    private static byte[] RandomBytes(int length)
    {
        var bytes = new byte[length];
        new Random(42).NextBytes(bytes);
        return bytes;
    }

    // ---- TODO-00 ----

    [Fact]
    public void ShouldProduceACompleteGzipFileThatRoundTrips()
    {
        var data = Repetitive();

        var compressed = Codecs.Gzip(data);

        Assert.Equal(0x1F, compressed[0]);
        Assert.Equal(0x8B, compressed[1]);
        Assert.Equal(data, Codecs.Gunzip(compressed));
    }

    [Fact]
    public void ShouldShrinkRepetitiveDataAndNotShrinkRandomData()
    {
        var repetitive = Repetitive();
        var random = RandomBytes(20_000);

        Assert.True(Codecs.Gzip(repetitive).Length < repetitive.Length / 10);
        Assert.True(Codecs.Gzip(random).Length >= random.Length);
    }

    [Fact]
    public void ShouldRejectDataThatIsNotGzip()
    {
        Assert.Throws<InvalidDataException>(() => Codecs.Gunzip(Encoding.UTF8.GetBytes("plain text, not gzip")));
    }

    // ---- TODO-01 ----

    [Theory]
    [InlineData(CompressionLevel.Fastest)]
    [InlineData(CompressionLevel.Optimal)]
    [InlineData(CompressionLevel.SmallestSize)]
    public void ShouldRoundTripBrotliAtEveryLevel(CompressionLevel level)
    {
        var data = Repetitive(500);

        var compressed = Codecs.BrotliCompress(data, level);

        Assert.True(compressed.Length < data.Length);
        Assert.Equal(data, Codecs.BrotliDecompress(compressed));
    }

    [Fact]
    public void ShouldBeSmallerWithBrotliThanGzipOnTextAtOptimal()
    {
        var data = Encoding.UTF8.GetBytes(string.Concat(Enumerable.Range(0, 3000)
            .Select(i => $"{{\"id\":{i},\"name\":\"user-{i % 50}\",\"status\":\"active\"}}\n")));

        Assert.True(Codecs.BrotliCompress(data, CompressionLevel.Optimal).Length < Codecs.Gzip(data).Length);
    }

    // ---- TODO-02 ----

    [Fact]
    public void ShouldCreateAZipWithAllEntries()
    {
        var files = new Dictionary<string, byte[]>
        {
            ["a.txt"] = Encoding.UTF8.GetBytes(new string('a', 10_000)),
            ["docs/b.txt"] = Encoding.UTF8.GetBytes("hello"),
            ["ação/ü.txt"] = Encoding.UTF8.GetBytes("utf8 name")
        };

        var zip = ZipTools.CreateZip(files);

        using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
        Assert.Equal(files.Keys.Order(), archive.Entries.Select(e => e.FullName).Order());
        var a = archive.GetEntry("a.txt")!;
        Assert.Equal(10_000, a.Length);
        Assert.True(a.CompressedLength < a.Length);
        Assert.Equal("b.txt", archive.GetEntry("docs/b.txt")!.Name);
    }

    [Fact]
    public void ShouldCreateAnEmptyButValidZip()
    {
        var zip = ZipTools.CreateZip(new Dictionary<string, byte[]>());

        using var archive = new ZipArchive(new MemoryStream(zip), ZipArchiveMode.Read);
        Assert.Empty(archive.Entries);
    }

    // ---- TODO-03 ----

    [Fact]
    public void ShouldReadBackWhatWasZipped()
    {
        var files = new Dictionary<string, byte[]>
        {
            ["a.txt"] = Repetitive(100),
            ["dir/b.bin"] = RandomBytes(500),
            ["ação/ü.txt"] = Encoding.UTF8.GetBytes("utf8")
        };

        var result = ZipTools.ReadZip(ZipTools.CreateZip(files), maxTotalBytes: 1_000_000);

        Assert.Equal(files.Count, result.Count);
        foreach (var (name, bytes) in files) Assert.Equal(bytes, result[name]);
    }

    [Fact]
    public void ShouldRefuseAnArchiveThatExpandsBeyondTheLimit()
    {
        var bomb = ZipTools.CreateZip(new Dictionary<string, byte[]> { ["zeros.bin"] = new byte[5_000_000] });

        Assert.True(bomb.Length < 50_000); // tiny on the wire
        Assert.Throws<InvalidDataException>(() => ZipTools.ReadZip(bomb, maxTotalBytes: 1_000_000));
    }

    [Fact]
    public void ShouldCountTheLimitAcrossAllEntries()
    {
        var zip = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            ["one.bin"] = new byte[600_000],
            ["two.bin"] = new byte[600_000]
        });

        Assert.Throws<InvalidDataException>(() => ZipTools.ReadZip(zip, maxTotalBytes: 1_000_000));
        Assert.Equal(2, ZipTools.ReadZip(zip, maxTotalBytes: 1_200_000).Count);
    }

    // ---- TODO-04 ----

    [Fact]
    public void ShouldExtractNormalEntriesIncludingSubfolders()
    {
        var zip = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            ["ok/a.txt"] = Encoding.UTF8.GetBytes("A"),
            ["top.txt"] = Encoding.UTF8.GetBytes("T")
        });
        var dest = Path.Combine(_dir, "out");

        ZipTools.ExtractSafely(zip, dest);

        Assert.Equal("A", File.ReadAllText(Path.Combine(dest, "ok", "a.txt")));
        Assert.Equal("T", File.ReadAllText(Path.Combine(dest, "top.txt")));
    }

    [Fact]
    public void ShouldBlockZipSlipWithParentSegments()
    {
        var zip = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            ["../escaped.txt"] = Encoding.UTF8.GetBytes("x")
        });
        var dest = Path.Combine(_dir, "out");

        Assert.Throws<InvalidDataException>(() => ZipTools.ExtractSafely(zip, dest));
        Assert.False(File.Exists(Path.Combine(_dir, "escaped.txt")));
    }

    [Fact]
    public void ShouldBlockAbsolutePathsAndSiblingFoldersWithTheSamePrefix()
    {
        var dest = Path.Combine(_dir, "out");
        var absolute = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            [Path.Combine(_dir, "abs-escaped.txt")] = Encoding.UTF8.GetBytes("x")
        });
        var sibling = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            ["../out-evil/x.txt"] = Encoding.UTF8.GetBytes("x")
        });

        Assert.Throws<InvalidDataException>(() => ZipTools.ExtractSafely(absolute, dest));
        Assert.Throws<InvalidDataException>(() => ZipTools.ExtractSafely(sibling, dest));
        Assert.False(File.Exists(Path.Combine(_dir, "abs-escaped.txt")));
        Assert.False(Directory.Exists(Path.Combine(_dir, "out-evil")));
    }

    [Fact]
    public void ShouldWriteNothingWhenAnyEntryIsMalicious()
    {
        var zip = ZipTools.CreateZip(new Dictionary<string, byte[]>
        {
            ["ok/a.txt"] = Encoding.UTF8.GetBytes("A"),
            ["../escaped.txt"] = Encoding.UTF8.GetBytes("x")
        });
        var dest = Path.Combine(_dir, "out");

        Assert.Throws<InvalidDataException>(() => ZipTools.ExtractSafely(zip, dest));

        Assert.False(File.Exists(Path.Combine(dest, "ok", "a.txt")));
    }

    // ---- TODO-05 ----

    [Fact]
    public void ShouldRoundTripATarGz()
    {
        var files = new Dictionary<string, byte[]>
        {
            ["app/config.json"] = Encoding.UTF8.GetBytes("{\"a\":1}"),
            ["app/data.bin"] = RandomBytes(3000),
            ["README"] = Repetitive(50)
        };

        var tarGz = TarTools.CreateTarGz(files);
        var result = TarTools.ReadTarGz(tarGz);

        Assert.Equal(0x1F, tarGz[0]);
        Assert.Equal(0x8B, tarGz[1]);
        Assert.Equal(files.Count, result.Count);
        foreach (var (name, bytes) in files) Assert.Equal(bytes, result[name]);
    }

    [Fact]
    public void ShouldWriteTarEntriesThatTarReaderUnderstandsAfterGunzip()
    {
        var tarGz = TarTools.CreateTarGz(new Dictionary<string, byte[]> { ["x.txt"] = Encoding.UTF8.GetBytes("hi") });

        using var gzip = new GZipStream(new MemoryStream(tarGz), CompressionMode.Decompress);
        using var reader = new System.Formats.Tar.TarReader(gzip);
        var entry = reader.GetNextEntry()!;

        Assert.Equal("x.txt", entry.Name);
        Assert.Equal(System.Formats.Tar.TarEntryFormat.Pax, entry.Format);
        Assert.Null(reader.GetNextEntry());
    }

    [Fact]
    public void ShouldShrinkARepetitiveTarGz()
    {
        var data = Repetitive();

        var tarGz = TarTools.CreateTarGz(new Dictionary<string, byte[]> { ["log.txt"] = data });

        Assert.True(tarGz.Length < data.Length / 5);
    }
}
