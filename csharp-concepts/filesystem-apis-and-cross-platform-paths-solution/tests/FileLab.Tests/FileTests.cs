namespace FileLab.Tests;

public class FileTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    public FileTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        if (Directory.Exists(_dir))
            Directory.Delete(_dir, recursive: true);
    }

    private string Touch(params string[] parts)
    {
        var path = Path.Combine([_dir, .. parts]);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, "x");
        return path;
    }

    // TODO-00

    [Fact]
    public void ShouldKeepAPlainRelativePieceInsideTheBase()
    {
        var result = FileOps.SafeJoin(_dir, Path.Combine("reports", "a.txt"));

        Assert.Equal(Path.Combine(Path.GetFullPath(_dir), "reports", "a.txt"), result);
    }

    [Fact]
    public void ShouldTreatARootedPieceAsRelativeInsteadOfReplacingTheBase()
    {
        var rooted = Path.DirectorySeparatorChar + Path.Combine("etc", "passwd");

        var result = FileOps.SafeJoin(_dir, rooted);

        Assert.Equal(Path.Combine(Path.GetFullPath(_dir), "etc", "passwd"), result);
    }

    [Theory]
    [InlineData("..")]
    [InlineData("../secret.txt")]
    [InlineData("a/../../secret.txt")]
    public void ShouldRejectTraversalOutOfTheBase(string input)
    {
        Assert.Throws<InvalidOperationException>(() => FileOps.SafeJoin(_dir, input));
    }

    [Fact]
    public void ShouldRejectASiblingFolderThatSharesThePrefix()
    {
        var name = Path.GetFileName(_dir);

        Assert.Throws<InvalidOperationException>(() => FileOps.SafeJoin(_dir, "../" + name + "2/x.txt"));
    }

    // TODO-01

    [Fact]
    public void ShouldFindFilesRecursivelyIgnoringCase()
    {
        Touch("a.log");
        Touch("sub", "b.LOG");
        Touch("sub", "deeper", "c.Log");
        Touch("sub", "d.txt");

        var found = FileOps.FindFiles(_dir, "*.log", maxDepth: 10);

        Assert.Equal(
            [
                "a.log",
                Path.Combine("sub", "b.LOG"),
                Path.Combine("sub", "deeper", "c.Log"),
            ],
            found);
    }

    [Fact]
    public void ShouldRespectTheMaximumDepth()
    {
        Touch("a.log");
        Touch("sub", "b.log");
        Touch("sub", "deeper", "c.log");

        Assert.Equal(["a.log"], FileOps.FindFiles(_dir, "*.log", maxDepth: 0));
        Assert.Equal(
            ["a.log", Path.Combine("sub", "b.log")],
            FileOps.FindFiles(_dir, "*.log", maxDepth: 1));
    }

    // TODO-02

    private bool TryLinkDirectory(string link, string target)
    {
        try
        {
            Directory.CreateSymbolicLink(link, target);
            return true;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            return false; // Windows without privileges or developer mode: nothing to verify here.
        }
    }

    [Fact]
    public void ShouldCountEachFileOnceWhenNoLinksExist()
    {
        Touch("deep.log");
        Touch("sub", "deep.log");

        Assert.Equal(2, FileOps.CountByName(_dir, "deep.log", followLinks: true));
        Assert.Equal(2, FileOps.CountByName(_dir, "deep.log", followLinks: false));
    }

    [Fact]
    public void ShouldFollowDirectoryLinksOnlyWhenAsked()
    {
        Touch("sub", "deep.log");
        if (!TryLinkDirectory(Path.Combine(_dir, "alias"), Path.Combine(_dir, "sub")))
            return;

        Assert.Equal(2, FileOps.CountByName(_dir, "deep.log", followLinks: true));
        Assert.Equal(1, FileOps.CountByName(_dir, "deep.log", followLinks: false));
    }

    // TODO-03

    [Fact]
    public void ShouldCreateTheFileWithAllLines()
    {
        var path = Path.Combine(_dir, "app.cfg");

        FileOps.SaveAtomically(path, ["one", "two"]);

        Assert.Equal(["one", "two"], File.ReadAllLines(path));
    }

    [Fact]
    public void ShouldReplaceExistingContentAndLeaveNoTempFile()
    {
        var path = Path.Combine(_dir, "app.cfg");
        File.WriteAllText(path, "old content that is much longer than the new one");

        FileOps.SaveAtomically(path, ["new"]);

        Assert.Equal(["new"], File.ReadAllLines(path));
        Assert.Equal([path], Directory.GetFiles(_dir));
    }

    [Fact]
    public void ShouldKeepTheOldFileAndCleanUpWhenWritingFailsHalfway()
    {
        var path = Path.Combine(_dir, "app.cfg");
        File.WriteAllText(path, "old");

        static IEnumerable<string> Crashing()
        {
            yield return "partial";
            throw new InvalidDataException("boom");
        }

        Assert.Throws<InvalidDataException>(() => FileOps.SaveAtomically(path, Crashing()));

        Assert.Equal("old", File.ReadAllText(path));
        Assert.Equal([path], Directory.GetFiles(_dir));
    }

    // TODO-04

    [Fact]
    public void ShouldRefuseASecondExclusiveOpenWhileTheFirstIsHeld()
    {
        var path = Touch("lock.txt");

        using var first = FileOps.TryOpenExclusive(path);
        using var second = FileOps.TryOpenExclusive(path);

        Assert.NotNull(first);
        Assert.Null(second);
    }

    [Fact]
    public void ShouldAllowOpeningAgainAfterTheFirstIsDisposed()
    {
        var path = Touch("lock.txt");
        FileOps.TryOpenExclusive(path)!.Dispose();

        using var again = FileOps.TryOpenExclusive(path);

        Assert.NotNull(again);
    }

    [Fact]
    public void ShouldNotHideAMissingFile()
    {
        Assert.Throws<FileNotFoundException>(() => FileOps.TryOpenExclusive(Path.Combine(_dir, "nope.txt")));
    }

    // TODO-05

    [Fact]
    public void ShouldReadBackWhatWasWrittenAtEachIndex()
    {
        using var file = new RecordFile(Path.Combine(_dir, "rec.bin"));

        for (var i = 0; i < 4; i++)
            file.Write(i, i * 100);

        Assert.Equal(4, file.Count);
        Assert.Equal(200, file.Read(2));
        Assert.Equal(0, file.Read(0));
    }

    [Fact]
    public void ShouldOverwriteOneRecordWithoutTouchingTheOthers()
    {
        using var file = new RecordFile(Path.Combine(_dir, "rec.bin"));
        for (var i = 0; i < 3; i++)
            file.Write(i, i + 1);

        file.Write(1, -1);

        Assert.Equal([1, -1, 3], new[] { file.Read(0), file.Read(1), file.Read(2) });
        Assert.Equal(3, file.Count);
    }

    [Fact]
    public void ShouldGrowTheFileWithZeroFilledGapWhenWritingPastTheEnd()
    {
        using var file = new RecordFile(Path.Combine(_dir, "rec.bin"));

        file.Write(3, 7);

        Assert.Equal(4, file.Count);
        Assert.Equal(0, file.Read(1));
        Assert.Equal(7, file.Read(3));
    }

    [Fact]
    public void ShouldRejectInvalidIndexes()
    {
        using var file = new RecordFile(Path.Combine(_dir, "rec.bin"));
        file.Write(0, 1);

        Assert.Throws<ArgumentOutOfRangeException>(() => file.Read(-1));
        Assert.Throws<ArgumentOutOfRangeException>(() => file.Read(1));
        Assert.Throws<ArgumentOutOfRangeException>(() => file.Write(-1, 5));
    }

    [Fact]
    public void ShouldSupportParallelWritesToDifferentIndexes()
    {
        using var file = new RecordFile(Path.Combine(_dir, "rec.bin"));
        file.Write(199, 0); // pre-size the file

        Parallel.For(0, 200, i => file.Write(i, i * 3));

        for (var i = 0; i < 200; i++)
            Assert.Equal(i * 3, file.Read(i));
    }

    [Fact]
    public void ShouldReopenAndSeeThePersistedRecords()
    {
        var path = Path.Combine(_dir, "rec.bin");
        using (var file = new RecordFile(path))
            file.Write(1, 42);

        using var reopened = new RecordFile(path);

        Assert.Equal(2, reopened.Count);
        Assert.Equal(42, reopened.Read(1));
    }
}
