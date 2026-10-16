using System.Xml.Linq;

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
    {
        var config = new XElement("configuration",
            new XElement("packageSources",
                new XElement("clear"),
                sources.Select(s => new XElement("add",
                    new XAttribute("key", s.Key), new XAttribute("value", s.Value)))),
            new XElement("packageSourceMapping",
                mappings.Select(m => new XElement("packageSource",
                    new XAttribute("key", m.Key),
                    m.Value.Select(p => new XElement("package", new XAttribute("pattern", p)))))));
        return config.ToString();
    }

    // TODO-05: Return the key of the source allowed to supply 'packageId', or null when no pattern
    // matches. A pattern is an exact id (no '*') or a prefix ending in '*' ("Contoso.*"), and a
    // lone "*" matches everything. Ids compare case-insensitively. Rule: an exact id beats any
    // prefix, a longer prefix beats a shorter one, and "*" is the weakest.
    public static string? Resolve(string nugetConfigXml, string packageId)
    {
        var doc = XDocument.Parse(nugetConfigXml);
        string? best = null;
        var bestScore = -1;
        foreach (var source in doc.Descendants("packageSourceMapping").Descendants("packageSource"))
        {
            foreach (var pattern in source.Elements("package").Select(p => (string?)p.Attribute("pattern")))
            {
                if (pattern is null) continue;
                var score = Score(pattern, packageId);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = (string?)source.Attribute("key");
                }
            }
        }
        return best;
    }

    private static int Score(string pattern, string id)
    {
        if (!pattern.EndsWith('*'))
            return string.Equals(pattern, id, StringComparison.OrdinalIgnoreCase) ? int.MaxValue : -1;
        var prefix = pattern[..^1];
        return id.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? prefix.Length : -1;
    }

    // TODO-07 (optional): Return the ids that no pattern matches. With any mapping present, restore
    // fails for these ids (NU1100) because no source is considered for them.
    public static IReadOnlyList<string> Unmapped(string nugetConfigXml, IEnumerable<string> packageIds)
        => packageIds.Where(id => Resolve(nugetConfigXml, id) is null).ToList();
}
