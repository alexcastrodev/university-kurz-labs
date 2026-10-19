using System.Text.RegularExpressions;
using static PublishingLab.Tests.PublishSandbox;

namespace PublishingLab.Tests;

public class PublishingTests
{
    // ---- Evaluation tests: `dotnet msbuild -getProperty`, no restore, no publish ----

    [Fact] // TODO-00
    public void FrameworkDependent_NeedsNoRuntimeIdentifier_AndHasNoAppHost()
    {
        using var sb = new PublishSandbox();
        // The RID is passed on the command line only to prove it does not imply self-contained.
        var r = sb.Evaluate(ProjectFiles.FrameworkDependentProject(),
            ["OutputType", "TargetFramework", "SelfContained", "UseAppHost", "PublishSingleFile", "PublishTrimmed"], [],
            "-property:RuntimeIdentifier=linux-x64");

        Assert.Equal("Exe", Prop(r, "OutputType"));
        Assert.Equal("net10.0", Prop(r, "TargetFramework"));
        Assert.Equal("false", Prop(r, "SelfContained"));
        Assert.Equal("false", Prop(r, "UseAppHost"));
        Assert.NotEqual("true", Prop(r, "PublishSingleFile"));
        Assert.NotEqual("true", Prop(r, "PublishTrimmed"));
    }

    [Fact] // TODO-01
    public void SelfContainedSingleFile_NeedsRid_AndTurnsOnTheSingleFileAnalyzer()
    {
        using var sb = new PublishSandbox();
        var r = sb.Evaluate(ProjectFiles.SelfContainedSingleFileProject("linux-x64"),
            ["RuntimeIdentifier", "SelfContained", "PublishSingleFile", "IncludeNativeLibrariesForSelfExtract",
             "EnableSingleFileAnalyzer", "EnableTrimAnalyzer", "PublishTrimmed"], []);

        Assert.Equal("linux-x64", Prop(r, "RuntimeIdentifier"));
        Assert.Equal("true", Prop(r, "SelfContained"));
        Assert.Equal("true", Prop(r, "PublishSingleFile"));
        Assert.Equal("true", Prop(r, "IncludeNativeLibrariesForSelfExtract"));
        // Inferred by the SDK from PublishSingleFile, not written by you.
        Assert.Equal("true", Prop(r, "EnableSingleFileAnalyzer"));
        Assert.NotEqual("true", Prop(r, "EnableTrimAnalyzer"));
        Assert.NotEqual("true", Prop(r, "PublishTrimmed"));
    }

    [Fact] // TODO-02
    public void Trimmed_UsesTrimMode_RootAssemblies_AndTurnsOnTheTrimAnalyzer()
    {
        using var sb = new PublishSandbox();
        var r = sb.Evaluate(ProjectFiles.TrimmedProject("osx-arm64", "full", "Plugins", "Billing"),
            ["RuntimeIdentifier", "SelfContained", "PublishTrimmed", "TrimMode", "EnableTrimAnalyzer", "EnableSingleFileAnalyzer"],
            ["TrimmerRootAssembly"]);

        Assert.Equal("osx-arm64", Prop(r, "RuntimeIdentifier"));
        Assert.Equal("true", Prop(r, "SelfContained"));
        Assert.Equal("true", Prop(r, "PublishTrimmed"));
        Assert.Equal("full", Prop(r, "TrimMode"));
        Assert.Equal("true", Prop(r, "EnableTrimAnalyzer"));
        Assert.NotEqual("true", Prop(r, "EnableSingleFileAnalyzer"));
        Assert.Equal(["Billing", "Plugins"], Identities(r, "TrimmerRootAssembly").Order(StringComparer.Ordinal));
    }

    [Fact] // TODO-03
    public void SlimRuntime_SetsFeatureSwitchesAndKeepsHttp3()
    {
        using var sb = new PublishSandbox();
        var r = sb.Evaluate(ProjectFiles.SlimRuntimeProject("win-x64"),
            ["RuntimeIdentifier", "SelfContained", "PublishTrimmed", "InvariantGlobalization", "EventSourceSupport",
             "UseSystemResourceKeys", "Http3Support", "TrimmerSingleWarn"], []);

        Assert.Equal("win-x64", Prop(r, "RuntimeIdentifier"));
        Assert.Equal("true", Prop(r, "SelfContained"));
        Assert.Equal("true", Prop(r, "PublishTrimmed"));
        Assert.Equal("true", Prop(r, "InvariantGlobalization"));
        Assert.Equal("false", Prop(r, "EventSourceSupport"));
        Assert.Equal("true", Prop(r, "UseSystemResourceKeys"));
        Assert.Equal("true", Prop(r, "Http3Support"));
        Assert.Equal("false", Prop(r, "TrimmerSingleWarn"));
    }

    // ---- Analyzer tests: `dotnet build` of a framework-dependent project, read IL warnings ----

    private static string[] WarningIds(string output) =>
        Regex.Matches(output, @"warning (IL\d{4})").Select(m => m.Groups[1].Value).Distinct().Order().ToArray();

    [Fact] // TODO-04
    public void AnalyzerProject_ReportsTheFourClassicProblems()
    {
        using var sb = new PublishSandbox();
        var (code, output) = sb.Build(AnalyzerFiles.AnalyzerProject(), SampleCode.RiskyProgram);

        Assert.True(code == 0, output);
        Assert.Equal(["IL2026", "IL2057", "IL2070", "IL3000"], WarningIds(output));
    }

    [Fact] // TODO-05
    public void FixedProgram_BuildsWithoutAnyIlWarning_AndStillBehaves()
    {
        using var sb = new PublishSandbox();
        var program = AnalyzerFiles.FixedProgram();
        var (code, output) = sb.Build(AnalyzerFiles.AnalyzerProject(), program);

        Assert.True(code == 0, output);
        Assert.Empty(WarningIds(output));
        Assert.Contains("AppContext.BaseDirectory", program);
        Assert.DoesNotContain("Assembly.Location", program);
        Assert.DoesNotContain("Type.GetType", program);

        // The fixed program is framework-dependent, so running it needs no runtime pack.
        var (runCode, run) = sb.RunBuiltApp();
        Assert.True(runCode == 0, run);
        var lines = run.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Contains("type=Plugin", lines);
        Assert.Contains("methods=Print,get_Total,set_Total", lines);
        Assert.Contains("json={\"Total\":10}", lines);
        var baseLine = Assert.Single(lines, l => l.StartsWith("base="));
        Assert.True(Directory.Exists(baseLine["base=".Length..]), baseLine);
    }

    // ---- Bonus ----

    [Fact] // TODO-06 (optional)
    public void Bonus_ReadyToRunAndCompression()
    {
        using var sb = new PublishSandbox();
        var r = sb.Evaluate(ProjectFiles.ReadyToRunCompressedProject("linux-arm64"),
            ["RuntimeIdentifier", "SelfContained", "PublishSingleFile", "PublishReadyToRun", "EnableCompressionInSingleFile"], []);

        Assert.Equal("linux-arm64", Prop(r, "RuntimeIdentifier"));
        Assert.Equal("true", Prop(r, "SelfContained"));
        Assert.Equal("true", Prop(r, "PublishSingleFile"));
        Assert.Equal("true", Prop(r, "PublishReadyToRun"));
        Assert.Equal("true", Prop(r, "EnableCompressionInSingleFile"));
    }

    [Fact] // TODO-07 (optional)
    public void Bonus_StrictProject_FailsTheBuildOnTrimWarnings()
    {
        using var sb = new PublishSandbox();
        var (code, output) = sb.Build(AnalyzerFiles.StrictAnalyzerProject(), SampleCode.RiskyProgram);

        Assert.NotEqual(0, code);
        Assert.Contains("error IL2026", output);
        Assert.Contains("error IL2057", output);
        Assert.Contains("error IL2070", output);
    }
}
