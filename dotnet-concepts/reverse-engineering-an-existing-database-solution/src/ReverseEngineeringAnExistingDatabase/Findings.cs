using System.Text.RegularExpressions;

namespace ReverseEngineeringAnExistingDatabase;

/// <summary>Reads scaffolded source text (default settings, Fluent API) and reports what the scaffolder decided.</summary>
public static partial class Findings
{
    /// <summary>
    /// TODO-04: class names of the keyless entities (table without primary key, and the view),
    /// sorted, found through <c>HasNoKey()</c> in the context file.
    /// </summary>
    public static IReadOnlyList<string> KeylessEntities(IReadOnlyDictionary<string, string> files)
        => KeylessRegex().Matches(files["LegacyDb.cs"])
            .Select(m => m.Groups[1].Value)
            .Order()
            .ToList();

    /// <summary>
    /// TODO-04: properties the scaffolder typed as <c>byte[]</c>, as "Class.Property", sorted.
    /// The context file is not an entity, skip it.
    /// </summary>
    public static IReadOnlyList<string> ByteArrayProperties(IReadOnlyDictionary<string, string> files)
        => files.Where(f => f.Key != "LegacyDb.cs")
            .SelectMany(f =>
            {
                var cls = ClassRegex().Match(f.Value).Groups[1].Value;
                return BytesRegex().Matches(f.Value).Select(m => $"{cls}.{m.Groups[1].Value}");
            })
            .Order()
            .ToList();

    [GeneratedRegex(@"Entity<(\w+)>\(entity =>\s*\{\s*entity\s*\.HasNoKey\(\)")]
    private static partial Regex KeylessRegex();

    [GeneratedRegex(@"public partial class (\w+)")]
    private static partial Regex ClassRegex();

    [GeneratedRegex(@"public byte\[\]\??\s+(\w+)")]
    private static partial Regex BytesRegex();
}
