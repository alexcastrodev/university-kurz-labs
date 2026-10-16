namespace CentralPackageManagementLab.Tests;

public sealed class CentralPackageManagementTests : IDisposable
{
    private readonly TempRepo _repo = new();

    public void Dispose() => _repo.Dispose();

    private static readonly Dictionary<string, string> Versions = new()
    {
        ["Contoso.Core"] = "1.2.3",
        ["Contoso.Extras"] = "4.5.6",
    };

    [Fact]
    public void Directory_packages_props_enables_cpm_and_declares_versions()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.DirectoryPackagesProps(Versions));
        var project = _repo.Write("app/app.csproj", PackageFiles.ProjectFile(["Contoso.Core"]));

        var result = _repo.Evaluate(project, "PackageVersion", "ManagePackageVersionsCentrally");

        Assert.Equal("true", result.GetProperty("Properties").GetProperty("ManagePackageVersionsCentrally").GetString());
        Assert.Equal("1.2.3", TempRepo.Metadata(result, "PackageVersion", "Contoso.Core", "Version"));
        Assert.Equal("4.5.6", TempRepo.Metadata(result, "PackageVersion", "Contoso.Extras", "Version"));
    }

    [Fact]
    public void Project_file_references_packages_without_a_version()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.DirectoryPackagesProps(Versions));
        var project = _repo.Write("app/app.csproj", PackageFiles.ProjectFile(["Contoso.Core", "Contoso.Extras"]));

        var result = _repo.Evaluate(project, "PackageReference", "ManagePackageVersionsCentrally");

        Assert.True(TempRepo.HasItem(result, "PackageReference", "Contoso.Core"));
        Assert.True(TempRepo.HasItem(result, "PackageReference", "Contoso.Extras"));
        Assert.Null(TempRepo.Metadata(result, "PackageReference", "Contoso.Core", "Version"));
        Assert.Null(TempRepo.Metadata(result, "PackageReference", "Contoso.Core", "VersionOverride"));
    }

    [Fact]
    public void Version_override_is_set_only_for_the_requested_package()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.DirectoryPackagesProps(Versions));
        var overrides = new Dictionary<string, string> { ["Contoso.Extras"] = "3.0.0" };
        var project = _repo.Write("app/app.csproj", PackageFiles.ProjectFile(["Contoso.Core", "Contoso.Extras"], overrides));

        var result = _repo.Evaluate(project, "PackageReference", "ManagePackageVersionsCentrally");

        Assert.Equal("3.0.0", TempRepo.Metadata(result, "PackageReference", "Contoso.Extras", "VersionOverride"));
        Assert.Null(TempRepo.Metadata(result, "PackageReference", "Contoso.Extras", "Version"));
        Assert.Null(TempRepo.Metadata(result, "PackageReference", "Contoso.Core", "VersionOverride"));
    }

    [Fact]
    public void Nested_props_imports_the_parent_versions_and_adds_its_own()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.DirectoryPackagesProps(Versions));
        _repo.Write("services/Directory.Packages.props",
            PackageFiles.NestedDirectoryPackagesProps(new Dictionary<string, string> { ["Contoso.Web"] = "7.0.0" }));
        var project = _repo.Write("services/api/api.csproj", PackageFiles.ProjectFile(["Contoso.Core", "Contoso.Web"]));

        var result = _repo.Evaluate(project, "PackageVersion", "ManagePackageVersionsCentrally");

        Assert.Equal("true", result.GetProperty("Properties").GetProperty("ManagePackageVersionsCentrally").GetString());
        Assert.Equal("1.2.3", TempRepo.Metadata(result, "PackageVersion", "Contoso.Core", "Version"));
        Assert.Equal("7.0.0", TempRepo.Metadata(result, "PackageVersion", "Contoso.Web", "Version"));
    }

    [Fact]
    public void Global_package_references_reach_projects_as_build_only_dependencies()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.GlobalPackagesProps(
            Versions, new Dictionary<string, string> { ["Contoso.Analyzers"] = "2.0.0" }));
        var project = _repo.Write("app/app.csproj", PackageFiles.ProjectFile(["Contoso.Core"]));

        var result = _repo.Evaluate(project, "PackageReference,GlobalPackageReference,PackageVersion", "ManagePackageVersionsCentrally");

        Assert.Equal("2.0.0", TempRepo.Metadata(result, "GlobalPackageReference", "Contoso.Analyzers", "Version"));
        Assert.True(TempRepo.HasItem(result, "PackageReference", "Contoso.Analyzers"));
        Assert.Equal("All", TempRepo.Metadata(result, "PackageReference", "Contoso.Analyzers", "PrivateAssets"));
        Assert.Equal("1.2.3", TempRepo.Metadata(result, "PackageVersion", "Contoso.Core", "Version"));
    }

    [Fact]
    public void Transitive_pinning_property_is_enabled_and_versions_are_kept()
    {
        _repo.Write("Directory.Packages.props", PackageFiles.TransitivePinningProps(Versions));
        var project = _repo.Write("app/app.csproj", PackageFiles.ProjectFile(["Contoso.Core"]));

        var result = _repo.Evaluate(project, "PackageVersion", "ManagePackageVersionsCentrally,CentralPackageTransitivePinningEnabled");

        var props = result.GetProperty("Properties");
        Assert.Equal("true", props.GetProperty("ManagePackageVersionsCentrally").GetString());
        Assert.Equal("true", props.GetProperty("CentralPackageTransitivePinningEnabled").GetString());
        Assert.Equal("4.5.6", TempRepo.Metadata(result, "PackageVersion", "Contoso.Extras", "Version"));
    }
}
