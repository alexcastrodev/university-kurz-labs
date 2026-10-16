using System.Text;

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
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project>");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in versions)
            sb.AppendLine($"    <PackageVersion Include=\"{id}\" Version=\"{version}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        return sb.ToString();
    }

    // TODO-01: Build a .csproj (Microsoft.NET.Sdk, net10.0) with one <PackageReference> per id and
    // NO Version attribute, so the central version is used. For ids present in 'versionOverrides'
    // (id -> version), add VersionOverride="..." instead.
    public static string ProjectFile(
        IEnumerable<string> packageIds,
        IReadOnlyDictionary<string, string>? versionOverrides = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <TargetFramework>net10.0</TargetFramework>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        foreach (var id in packageIds)
        {
            if (versionOverrides is not null && versionOverrides.TryGetValue(id, out var v))
                sb.AppendLine($"    <PackageReference Include=\"{id}\" VersionOverride=\"{v}\" />");
            else
                sb.AppendLine($"    <PackageReference Include=\"{id}\" />");
        }
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        return sb.ToString();
    }

    // TODO-02: Build the Directory.Packages.props of a NESTED folder. MSBuild only imports the
    // nearest one, so it must import the parent file with
    // $([MSBuild]::GetPathOfFileAbove(Directory.Packages.props, $(MSBuildThisFileDirectory)..))
    // and then add its own <PackageVersion> items (id -> version) from 'versions'.
    public static string NestedDirectoryPackagesProps(IReadOnlyDictionary<string, string> versions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project>");
        sb.AppendLine("  <Import Project=\"$([MSBuild]::GetPathOfFileAbove(Directory.Packages.props, $(MSBuildThisFileDirectory)..))\" />");
        sb.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in versions)
            sb.AppendLine($"    <PackageVersion Include=\"{id}\" Version=\"{version}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        return sb.ToString();
    }

    // TODO-03: Like TODO-00, but also add one <GlobalPackageReference Include Version> per entry of
    // 'globalPackages' (id -> version). Those packages reach every project as build-only
    // dependencies without any PackageReference in the project file.
    public static string GlobalPackagesProps(
        IReadOnlyDictionary<string, string> versions,
        IReadOnlyDictionary<string, string> globalPackages)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project>");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in versions)
            sb.AppendLine($"    <PackageVersion Include=\"{id}\" Version=\"{version}\" />");
        foreach (var (id, version) in globalPackages)
            sb.AppendLine($"    <GlobalPackageReference Include=\"{id}\" Version=\"{version}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        return sb.ToString();
    }

    // TODO-06 (optional): Like TODO-00, but also enable transitive pinning
    // (CentralPackageTransitivePinningEnabled = true). 'versions' may then contain packages that no
    // project references directly: those are pinned when they appear transitively.
    public static string TransitivePinningProps(IReadOnlyDictionary<string, string> versions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<Project>");
        sb.AppendLine("  <PropertyGroup>");
        sb.AppendLine("    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>");
        sb.AppendLine("    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>");
        sb.AppendLine("  </PropertyGroup>");
        sb.AppendLine("  <ItemGroup>");
        foreach (var (id, version) in versions)
            sb.AppendLine($"    <PackageVersion Include=\"{id}\" Version=\"{version}\" />");
        sb.AppendLine("  </ItemGroup>");
        sb.AppendLine("</Project>");
        return sb.ToString();
    }
}
