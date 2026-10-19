namespace ReverseEngineeringAnExistingDatabase;

/// <summary>Reads scaffolded source text (default settings, Fluent API) and reports what the scaffolder decided.</summary>
public static class Findings
{
    /// <summary>
    /// TODO-04: class names of the keyless entities (table without primary key, and the view),
    /// sorted, found through <c>HasNoKey()</c> in the context file.
    /// </summary>
    public static IReadOnlyList<string> KeylessEntities(IReadOnlyDictionary<string, string> files)
        => throw new NotImplementedException("TODO-04");

    /// <summary>
    /// TODO-04: properties the scaffolder typed as <c>byte[]</c>, as "Class.Property", sorted.
    /// The context file is not an entity, skip it.
    /// </summary>
    public static IReadOnlyList<string> ByteArrayProperties(IReadOnlyDictionary<string, string> files)
        => throw new NotImplementedException("TODO-04");
}
