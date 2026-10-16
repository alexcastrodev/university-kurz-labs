namespace FileLab;

public static class FileOps
{
    public static string SafeJoin(string baseDir, string userInput)
    {
        // TODO-00: Build the full path of userInput inside baseDir and return it (already resolved
        //   with Path.GetFullPath). Use Path.Join, not Path.Combine: a rooted userInput such as
        //   "/etc/passwd" must be treated as a relative piece, not replace baseDir.
        //   Then confine the result: if it is not inside baseDir (watch out for ".." and for a
        //   sibling folder like "<base>2"), throw InvalidOperationException.
        //   Tip: compare against the full base path with a trailing directory separator, using
        //   StringComparison.Ordinal.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static IReadOnlyList<string> FindFiles(string root, string pattern, int maxDepth)
    {
        // TODO-01: Return the files under root matching the pattern, as paths RELATIVE to root
        //   (Path.GetRelativePath), sorted with StringComparer.Ordinal.
        //   Use Directory.EnumerateFiles with an EnumerationOptions that recurses, matches
        //   case-insensitively (do not rely on the platform default) and honors MaxRecursionDepth.
        //   maxDepth 0 means only the files directly inside root.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static int CountByName(string root, string fileName, bool followLinks)
    {
        // TODO-02: Count how many files named fileName exist anywhere under root (recursive).
        //   When followLinks is false, symbolic links must not be walked or counted: use
        //   EnumerationOptions.AttributesToSkip with FileAttributes.ReparsePoint.
        //   When true, the default behavior (links are followed) applies.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void SaveAtomically(string path, IEnumerable<string> lines)
    {
        // TODO-03: Write the lines to path so a failure in the middle never damages the old file.
        //   Write to a temp file in the SAME directory as path (Path.GetRandomFileName() + FileMode.CreateNew),
        //   then swap it in with File.Move(tmp, path, overwrite: true).
        //   If enumerating or writing throws, delete the temp file and let the exception propagate.
        //   When it finishes, the directory must contain no leftover temp file.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static FileStream? TryOpenExclusive(string path)
    {
        // TODO-04: Open the existing file for reading and writing with FileShare.None.
        //   Return the stream, or null if another open handle already holds the file
        //   (catch IOException). Do not swallow FileNotFoundException.
        throw new NotImplementedException("Not implemented yet.");
    }
}
