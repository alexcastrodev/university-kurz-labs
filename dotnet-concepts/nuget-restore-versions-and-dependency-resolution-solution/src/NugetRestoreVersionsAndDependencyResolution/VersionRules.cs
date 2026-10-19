using NuGet.Versioning;

namespace NugetRestoreVersionsAndDependencyResolution;

public static class VersionRules
{
    public static int Compare(string left, string right)
    {
        // TODO-00: SemVer 2.0 ordering with NuGet's comparer: parse both with NuGetVersion.Parse and
        //   compare with VersionComparer.Default. Return the sign only (-1, 0, 1).
        //   1.0.0-beta.2 < 1.0.0, 1.0.0-beta.2 < 1.0.0-beta.10 (numeric identifiers compare as numbers),
        //   1.0 == 1.0.0, and build metadata (+abc) is ignored.
        return Math.Sign(VersionComparer.Default.Compare(NuGetVersion.Parse(left), NuGetVersion.Parse(right)));
    }

    public static bool Satisfies(string version, string range)
    {
        // TODO-00 (continued): VersionRange.Parse(range).Satisfies(NuGetVersion.Parse(version)).
        //   A bare "1.0" means ">= 1.0", "[1.0]" is exact, "(1.0,2.0)" excludes both ends, "[1.0,2.0)" includes the lower one.
        return VersionRange.Parse(range).Satisfies(NuGetVersion.Parse(version));
    }
}
