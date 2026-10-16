using NuGet.Versioning;

namespace NugetRestoreVersionsAndDependencyResolution;

/// <summary>A dependency edge: package id plus the version range exactly as written in a PackageReference.</summary>
public sealed record Dep(string Id, string Range)
{
    public VersionRange Parsed => VersionRange.Parse(Range);
}

/// <summary>In-memory package feed: id -> version -> dependencies. Ready to use, nothing to implement here.</summary>
public sealed class Feed
{
    private readonly Dictionary<string, Dictionary<NuGetVersion, Dep[]>> _packages =
        new(StringComparer.OrdinalIgnoreCase);

    public Feed Add(string id, string version, params Dep[] dependencies)
    {
        if (!_packages.TryGetValue(id, out var versions))
            _packages[id] = versions = new Dictionary<NuGetVersion, Dep[]>();
        versions[NuGetVersion.Parse(version)] = dependencies;
        return this;
    }

    public IReadOnlyList<NuGetVersion> Versions(string id) =>
        _packages.TryGetValue(id, out var v) ? v.Keys.ToList() : [];

    public IReadOnlyList<Dep> DependenciesOf(string id, NuGetVersion version) =>
        _packages.TryGetValue(id, out var v) && v.TryGetValue(version, out var d) ? d : [];
}
