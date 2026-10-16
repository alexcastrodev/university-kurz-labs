namespace CentralPackageManagementLab;

/// <summary>
/// Builds the text of the MSBuild files behind Central Package Management.
/// The tests write these strings to disk and let MSBuild evaluate them.
/// </summary>
public static class PackageFiles
{
    // TODO-00: Build a Directory.Packages.props that turns Central Package Management on
    // (ManagePackageVersionsCentrally = true) and declares one <PackageVersion Include Version>
    // item per entry of 'versions' (package id -> version).
    public static string DirectoryPackagesProps(IReadOnlyDictionary<string, string> versions)
        => throw new NotImplementedException();

    // TODO-01: Build a .csproj (Microsoft.NET.Sdk, net10.0) with one <PackageReference> per id and
    // NO Version attribute, so the central version is used. For ids present in 'versionOverrides'
    // (id -> version), add VersionOverride="..." instead.
    public static string ProjectFile(
        IEnumerable<string> packageIds,
        IReadOnlyDictionary<string, string>? versionOverrides = null)
        => throw new NotImplementedException();

    // TODO-02: Build the Directory.Packages.props of a NESTED folder. MSBuild only imports the
    // nearest one, so it must import the parent file with
    // $([MSBuild]::GetPathOfFileAbove(Directory.Packages.props, $(MSBuildThisFileDirectory)..))
    // and then add its own <PackageVersion> items (id -> version) from 'versions'.
    public static string NestedDirectoryPackagesProps(IReadOnlyDictionary<string, string> versions)
        => throw new NotImplementedException();

    // TODO-03: Like TODO-00, but also add one <GlobalPackageReference Include Version> per entry of
    // 'globalPackages' (id -> version). Those packages reach every project as build-only
    // dependencies without any PackageReference in the project file.
    public static string GlobalPackagesProps(
        IReadOnlyDictionary<string, string> versions,
        IReadOnlyDictionary<string, string> globalPackages)
        => throw new NotImplementedException();

    // TODO-06 (optional): Like TODO-00, but also enable transitive pinning
    // (CentralPackageTransitivePinningEnabled = true). 'versions' may then contain packages that no
    // project references directly: those are pinned when they appear transitively.
    public static string TransitivePinningProps(IReadOnlyDictionary<string, string> versions)
        => throw new NotImplementedException();
}
