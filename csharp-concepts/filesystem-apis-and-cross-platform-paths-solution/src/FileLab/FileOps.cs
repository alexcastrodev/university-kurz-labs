namespace FileLab;

public static class FileOps
{
    public static string SafeJoin(string baseDir, string userInput)
    {
        var baseFull = Path.GetFullPath(baseDir);
        if (!Path.EndsInDirectorySeparator(baseFull))
            baseFull += Path.DirectorySeparatorChar;

        // Join never discards baseDir, even when userInput is rooted.
        var full = Path.GetFullPath(Path.Join(baseDir, userInput));
        if (!full.StartsWith(baseFull, StringComparison.Ordinal))
            throw new InvalidOperationException("Path escapes the base folder");
        return full;
    }

    public static IReadOnlyList<string> FindFiles(string root, string pattern, int maxDepth)
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            MatchCasing = MatchCasing.CaseInsensitive,
            MaxRecursionDepth = maxDepth,
        };
        return Directory.EnumerateFiles(root, pattern, options)
            .Select(f => Path.GetRelativePath(root, f))
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    public static int CountByName(string root, string fileName, bool followLinks)
    {
        var options = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            MatchCasing = MatchCasing.CaseSensitive,
            AttributesToSkip = followLinks
                ? FileAttributes.Hidden | FileAttributes.System
                : FileAttributes.Hidden | FileAttributes.System | FileAttributes.ReparsePoint,
        };
        return Directory.EnumerateFiles(root, fileName, options).Count();
    }

    public static void SaveAtomically(string path, IEnumerable<string> lines)
    {
        var dir = Path.GetDirectoryName(Path.GetFullPath(path))!;
        // Same directory as the target, so the final move stays on one volume.
        var tmp = Path.Join(dir, Path.GetRandomFileName());
        try
        {
            using (var stream = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                foreach (var line in lines)
                    writer.WriteLine(line);
                writer.Flush();
                stream.Flush(true);
            }
            File.Move(tmp, path, overwrite: true);
        }
        catch
        {
            File.Delete(tmp);
            throw;
        }
    }

    public static FileStream? TryOpenExclusive(string path)
    {
        try
        {
            return new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (FileNotFoundException)
        {
            throw;
        }
        catch (IOException)
        {
            return null;
        }
    }
}
