namespace PackagingLab.Tests;

public class PackagingTests : IDisposable
{
    private readonly PackSandbox _box = new();

    public void Dispose() => _box.Dispose();

    private const string GreeterSource = "namespace Contoso { public class Greeter { public string Hi() => \"hi\"; } }";

    private Package PackOne(string csproj, string name = "Greeter.csproj", params string[] extra)
    {
        _box.Write(name, csproj);
        _box.Write("Greeter.cs", GreeterSource);
        _box.Pack(name, extra);
        return new Package(Directory.GetFiles(_box.OutDir, "*.nupkg").Single());
    }

    [Fact]
    public void Todo00_Metadata_EndsUpInTheNuspecAndTheAssemblyNameStaysSeparate()
    {
        var pkg = PackOne(PackagingFiles.LibraryProject());

        Assert.Equal("Contoso.Greeter", pkg.Meta("id"));
        Assert.Equal("1.2.3-beta.4", pkg.Meta("version"));
        Assert.Equal("Contoso", pkg.Meta("authors"));
        Assert.Equal("Greets people.", pkg.Meta("description"));
        Assert.Equal("expression", pkg.Attr("license", "type"));
        Assert.Equal("MIT", pkg.Meta("license"));
        Assert.Equal("https://example.com/greeter", pkg.Meta("projectUrl"));
        Assert.Equal("greeting sample", pkg.Meta("tags"));
        Assert.Equal("git", pkg.Attr("repository", "type"));
        Assert.Equal("https://example.com/greeter.git", pkg.Attr("repository", "url"));

        // PackageId names the package, AssemblyName names the DLL inside it.
        Assert.Contains("lib/net10.0/Greeter.dll", pkg.Entries);
        Assert.DoesNotContain("lib/net10.0/Contoso.Greeter.dll", pkg.Entries);
        Assert.Equal("Contoso.Greeter.1.2.3-beta.4.nupkg", _box.OutFiles().Single());
    }

    [Fact]
    public void Todo01_ReadmeIsDeclaredTwiceAndSymbolsGoIntoASeparateSnupkg()
    {
        _box.Write("README.md", PackagingFiles.ReadmeMarkdown);
        var pkg = PackOne(PackagingFiles.LibraryProjectWithReadmeAndSymbols());

        Assert.Equal("README.md", pkg.Meta("readme"));
        Assert.Contains("README.md", pkg.Entries);
        Assert.Equal(
            ["Contoso.Greeter.1.0.0.nupkg", "Contoso.Greeter.1.0.0.snupkg"],
            _box.OutFiles());
    }

    [Fact]
    public void Todo02_NotPackableProjectProducesNoPackage()
    {
        _box.Write("Tool.csproj", PackagingFiles.NotPackableProject());
        _box.Write("Greeter.cs", GreeterSource);

        _box.Pack("Tool.csproj");

        Assert.Empty(_box.OutFiles());
    }

    [Fact]
    public void Todo03_MultiTargetingCreatesOneLibFolderPerFramework()
    {
        var pkg = PackOne(PackagingFiles.MultiTargetProject());

        Assert.Contains("lib/net10.0/Greeter.dll", pkg.Entries);
        Assert.Contains("lib/netstandard2.1/Greeter.dll", pkg.Entries);
    }

    [Fact]
    public void Todo04_BuildTransitivePropsFlowToConsumersAtThePackageRoot()
    {
        _box.Write("Contoso.Greeter.props", PackagingFiles.BuildTransitiveProps());
        var pkg = PackOne(PackagingFiles.ProjectWithBuildTransitive());

        Assert.Contains("buildTransitive/Contoso.Greeter.props", pkg.Entries);
        Assert.Contains("<GreeterEnabled>true</GreeterEnabled>", PackagingFiles.BuildTransitiveProps());
    }

    [Fact]
    public void Todo05_ProjectReferenceBecomesADependencyNotABundledDll()
    {
        _box.Write("Contoso.Core/Contoso.Core.csproj", PackagingFiles.CoreProject());
        _box.Write("Contoso.Core/Core.cs", "namespace Contoso { public class Core { } }");
        _box.Write("Contoso.Extras/Contoso.Extras.csproj", PackagingFiles.DependentProject());
        _box.Write("Contoso.Extras/Extras.cs", "namespace Contoso { public class Extras { public Core C = new(); } }");

        _box.Pack("Contoso.Extras/Contoso.Extras.csproj");

        var pkg = new Package(Directory.GetFiles(_box.OutDir, "*.nupkg").Single());
        Assert.Contains("Contoso.Core 2.0.0", pkg.Dependencies());
        Assert.Contains("lib/net10.0/Contoso.Extras.dll", pkg.Entries);
        Assert.DoesNotContain("lib/net10.0/Contoso.Core.dll", pkg.Entries);
    }

    // Bonus

    [Fact]
    public void Todo06_ReproducibleBuildPropertiesAreEvaluatedAsTrue()
    {
        _box.Write("Lib.csproj", PackagingFiles.ReproducibleProject());

        var json = System.Text.Json.JsonDocument.Parse(_box.Run(
            ["msbuild", "Lib.csproj", "-getProperty:Deterministic,ContinuousIntegrationBuild,PublishRepositoryUrl,EmbedUntrackedSources"]))
            .RootElement.GetProperty("Properties");

        foreach (var name in new[] { "Deterministic", "ContinuousIntegrationBuild", "PublishRepositoryUrl", "EmbedUntrackedSources" })
            Assert.Equal("true", json.GetProperty(name).GetString()!.ToLowerInvariant());
    }

    [Fact]
    public void Todo07_GeneratePackageOnBuildPacksWithoutDotnetPack()
    {
        _box.Write("Greeter.csproj", PackagingFiles.PackOnBuildProject());
        _box.Write("Greeter.cs", GreeterSource);

        _box.Run(["build", "Greeter.csproj", "-c", "Release"]);

        Assert.True(File.Exists(Path.Combine(_box.Root, "bin", "Release", "Contoso.Greeter.1.0.0.nupkg")));
    }
}
