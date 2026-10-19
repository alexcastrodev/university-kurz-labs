namespace CentralPackageManagementLab;

/// <summary>
/// Package Source Mapping is resolved by NuGet at restore time, so it cannot be evaluated offline.
/// This class builds the nuget.config text and applies the documented matching rule by hand.
/// </summary>
public static class SourceMapping
{
    // TODO-04: Build a nuget.config with a <packageSources> section (one <add key value /> per entry
    // of 'sources', key -> url) and a <packageSourceMapping> section with one
    // <packageSource key="..."> per entry of 'mappings' (source key -> patterns), each pattern as
    // <package pattern="..." />. Start with <clear /> so only these sources are used.
    public static string NuGetConfig(
        IReadOnlyDictionary<string, string> sources,
        IReadOnlyDictionary<string, string[]> mappings)
        => throw new NotImplementedException();

    // TODO-05: Return the key of the source allowed to supply 'packageId', or null when no pattern
    // matches. A pattern is an exact id (no '*') or a prefix ending in '*' ("Contoso.*"), and a
    // lone "*" matches everything. Ids compare case-insensitively. Rule: an exact id beats any
    // prefix, a longer prefix beats a shorter one, and "*" is the weakest.
    public static string? Resolve(string nugetConfigXml, string packageId)
        => throw new NotImplementedException();


    // TODO-07 (optional): Return the ids that no pattern matches. With any mapping present, restore
    // fails for these ids (NU1100) because no source is considered for them.
    public static IReadOnlyList<string> Unmapped(string nugetConfigXml, IEnumerable<string> packageIds)
        => throw new NotImplementedException();
}
