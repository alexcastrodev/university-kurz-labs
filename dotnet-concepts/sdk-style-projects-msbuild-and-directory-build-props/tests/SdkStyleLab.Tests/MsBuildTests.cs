namespace SdkStyleLab.Tests;

public class MsBuildTests : IDisposable
{
    private readonly MsBuildSandbox _box = new();

    public void Dispose() => _box.Dispose();

    private const string Cs = "class C { }";

    [Fact]
    public void Todo00_ClassLibraryProject_UsesTheSdkAndGlobsCompileItems()
    {
        var project = _box.Write("lib/Lib.csproj", MsBuildFiles.ClassLibraryProject());
        _box.Write("lib/A.cs", Cs);
        _box.Write("lib/sub/deep/B.cs", Cs);
        _box.Write("lib/bin/Debug/Skip.cs", Cs);
        _box.Write("lib/obj/Skip2.cs", Cs);
        _box.Write("lib/Notes.txt", "not code");

        var result = _box.Evaluate(project, ["TargetFramework", "MSBuildProjectName"], ["Compile"]);

        Assert.Equal("net10.0", MsBuildSandbox.Prop(result, "TargetFramework"));
        Assert.Equal(["A.cs", "sub/deep/B.cs"], MsBuildSandbox.CompileItems(result));
        Assert.StartsWith("<Project Sdk=\"Microsoft.NET.Sdk\"", MsBuildFiles.ClassLibraryProject().TrimStart());
    }

    [Fact]
    public void Todo01_ProjectWithExcludedFile_RemovesLegacyFromTheGlob()
    {
        var project = _box.Write("lib/Lib.csproj", MsBuildFiles.ProjectWithExcludedFile());
        _box.Write("lib/A.cs", Cs);
        _box.Write("lib/sub/B.cs", Cs);
        _box.Write("lib/Legacy/Broken.cs", "#error broken");
        _box.Write("lib/Legacy/deeper/AlsoBroken.cs", "#error broken");

        var result = _box.Evaluate(project, [], ["Compile"]);

        Assert.Equal(["A.cs", "sub/B.cs"], MsBuildSandbox.CompileItems(result));
    }

    [Fact]
    public void Todo02_DirectoryBuildProps_AppliesToEveryProjectBelowAndTheProjectBodyWins()
    {
        _box.Write("Directory.Build.props", MsBuildFiles.DirectoryBuildProps());
        var plain = _box.Write("src/Plain/Plain.csproj", "<Project Sdk=\"Microsoft.NET.Sdk\" />");
        var overriding = _box.Write("src/Over/Over.csproj", MsBuildFiles.ProjectOverridingNullable());
        string[] props = ["TargetFramework", "Nullable", "LangVersion", "TreatWarningsAsErrors"];

        var inherited = _box.Evaluate(plain, props, []);
        var overridden = _box.Evaluate(overriding, props, []);

        Assert.Equal("net10.0", MsBuildSandbox.Prop(inherited, "TargetFramework"));
        Assert.Equal("enable", MsBuildSandbox.Prop(inherited, "Nullable"));
        Assert.Equal("14.0", MsBuildSandbox.Prop(inherited, "LangVersion"));
        Assert.Equal("true", MsBuildSandbox.Prop(inherited, "TreatWarningsAsErrors"));

        Assert.Equal("disable", MsBuildSandbox.Prop(overridden, "Nullable"));
        Assert.Equal("net10.0", MsBuildSandbox.Prop(overridden, "TargetFramework"));
        Assert.Equal("true", MsBuildSandbox.Prop(overridden, "TreatWarningsAsErrors"));
    }

    [Fact]
    public void Todo03_DirectoryBuildTargets_SeesValuesTheProjectChose()
    {
        _box.Write("Directory.Build.targets", MsBuildFiles.DirectoryBuildTargets());
        var exe = _box.Write("app/App.csproj",
            "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><OutputType>Exe</OutputType></PropertyGroup></Project>");
        var lib = _box.Write("lib/Lib.csproj",
            "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>");

        Assert.Equal("app", MsBuildSandbox.Prop(_box.Evaluate(exe, ["ProjectKind"], []), "ProjectKind"));
        Assert.Equal("library", MsBuildSandbox.Prop(_box.Evaluate(lib, ["ProjectKind"], []), "ProjectKind"));
    }

    [Fact]
    public void Todo04_NestedDirectoryBuildProps_ChainsToTheParentFile()
    {
        _box.Write("Directory.Build.props", MsBuildFiles.DirectoryBuildProps());
        _box.Write("tests/Directory.Build.props", MsBuildFiles.NestedDirectoryBuildProps());
        var project = _box.Write("tests/Unit/Unit.csproj", "<Project Sdk=\"Microsoft.NET.Sdk\" />");

        var result = _box.Evaluate(project, ["IsTestProject", "TargetFramework", "Nullable", "TreatWarningsAsErrors"], []);

        Assert.Equal("true", MsBuildSandbox.Prop(result, "IsTestProject"));
        Assert.Equal("net10.0", MsBuildSandbox.Prop(result, "TargetFramework"));
        Assert.Equal("enable", MsBuildSandbox.Prop(result, "Nullable"));
        Assert.Equal("true", MsBuildSandbox.Prop(result, "TreatWarningsAsErrors"));
    }

    [Fact]
    public void Todo05_CustomTargetProject_WritesTheStampFileWhenTheTargetRuns()
    {
        var project = _box.Write("lib/Stamped.csproj", MsBuildFiles.CustomTargetProject());

        _box.Run(["msbuild", project, "-t:WriteStamp"]);

        var stamp = File.ReadAllText(Path.Combine(_box.Root, "lib", "stamp.txt")).Trim();
        Assert.Equal("Stamped|net10.0", stamp);
    }

    [Fact]
    public void Todo06_Bonus_ReleaseStrictProject_IsStrictOnlyInRelease()
    {
        var project = _box.Write("lib/Lib.csproj", MsBuildFiles.ReleaseStrictProject());

        var debug = _box.Evaluate(project, ["TreatWarningsAsErrors"], []);
        var release = _box.Evaluate(project, ["TreatWarningsAsErrors"], [], "-property:Configuration=Release");

        Assert.NotEqual("true", MsBuildSandbox.Prop(debug, "TreatWarningsAsErrors"));
        Assert.Equal("true", MsBuildSandbox.Prop(release, "TreatWarningsAsErrors"));
    }

    [Fact]
    public void Todo07_Bonus_ExplicitCompileProject_ListsOnlyTheNamedFile()
    {
        var project = _box.Write("lib/Lib.csproj", MsBuildFiles.ExplicitCompileProject());
        _box.Write("lib/Program.cs", Cs);
        _box.Write("lib/Other.cs", Cs);

        var result = _box.Evaluate(project, [], ["Compile"]);

        Assert.Equal(["Program.cs"], MsBuildSandbox.CompileItems(result));
    }
}
