using System.Reflection;

namespace NativeAotLab.Tests;

public class AotTests : IDisposable
{
    private readonly AotSandbox _box = new();

    public void Dispose() => _box.Dispose();

    private static readonly string[] AnalyzerProps =
        ["EnableAotAnalyzer", "EnableTrimAnalyzer", "EnableSingleFileAnalyzer"];

    private BuildResult BuildSnippets(Dictionary<string, string> sources) =>
        _box.Build(AotSandbox.AnalyzerProject, sources);

    [Fact]
    public void Todo00_PublishAotProject_TurnsOnAnalyzersAndTrimming()
    {
        var text = AotFiles.PublishAotProject();
        string[] props = [.. AnalyzerProps, "PublishAot", "PublishTrimmed", "InvariantGlobalization", "OutputType"];

        var on = _box.Evaluate(text, props);
        Assert.Equal("true", on["PublishAot"]);
        Assert.Equal("Exe", on["OutputType"]);
        foreach (var p in AnalyzerProps) Assert.Equal("true", on[p]);
        Assert.Equal("true", on["PublishTrimmed"]);
        // PublishAot does not imply invariant globalization.
        Assert.Equal("", on["InvariantGlobalization"]);

        // The same project with the switch turned off proves PublishAot is what flips them.
        var off = _box.Evaluate(text, props, "-p:PublishAot=false");
        foreach (var p in AnalyzerProps) Assert.Equal("", off[p]);
        Assert.Equal("", off["PublishTrimmed"]);
    }

    [Fact]
    public void Todo01_AotCompatibleLibrary_TurnsOnAnalyzersWithoutPublishAot()
    {
        string[] props = [.. AnalyzerProps, "IsAotCompatible", "IsTrimmable", "PublishAot", "PublishTrimmed"];

        var r = _box.Evaluate(AotFiles.AotCompatibleLibrary(), props);

        Assert.Equal("true", r["IsAotCompatible"]);
        foreach (var p in AnalyzerProps) Assert.Equal("true", r[p]);
        Assert.Equal("true", r["IsTrimmable"]);
        // A library is not published as a native binary and nothing is trimmed at its build.
        Assert.Equal("", r["PublishAot"]);
        Assert.Equal("", r["PublishTrimmed"]);
    }

    [Fact]
    public void Todo02_WarningIds_MatchWhatTheAnalyzerReallyReports()
    {
        var build = BuildSnippets(new()
        {
            ["MakeGenericType.cs"] = Snippets.MakeGenericType,
            ["LoadFrom.cs"] = Snippets.LoadFrom,
            ["JsonReflection.cs"] = Snippets.JsonReflection,
        });

        string[] Real(string file) => build.In(file).Select(d => d.Id).Distinct().Order().ToArray();

        Assert.Equal(0, build.ExitCode);
        Assert.Equal(Real("MakeGenericType.cs"), AotFiles.WarningIdsForMakeGenericType().Order().ToArray());
        Assert.Equal(Real("LoadFrom.cs"), AotFiles.WarningIdsForLoadFrom().Order().ToArray());
        Assert.Equal(Real("JsonReflection.cs"), AotFiles.WarningIdsForJsonReflection().Order().ToArray());
        // Guard against an empty answer matching an empty result.
        Assert.NotEmpty(Real("MakeGenericType.cs"));
    }

    [Fact]
    public void Todo03_JsonSourceGeneration_HasNoWarningsAndRoundTrips()
    {
        var build = BuildSnippets(new() { ["Todo.cs"] = AotFiles.JsonSourceGeneratedSnippet() });

        Assert.True(build.ExitCode == 0, build.Output);
        Assert.Empty(build.Diagnostics);

        // The library really works: load it and call the generated serializer.
        var asm = Assembly.LoadFrom(build.Dll);
        var todo = asm.GetType("Todo")!;
        var json = asm.GetType("TodoJson")!;
        var value = Activator.CreateInstance(todo, 7, "write lab")!;
        var text = (string)json.GetMethod("ToJson")!.Invoke(null, [value])!;
        Assert.Equal("""{"Id":7,"Title":"write lab"}""", text);
        var back = json.GetMethod("FromJson")!.Invoke(null, [text]);
        Assert.Equal(value, back);
    }

    [Fact]
    public void Todo04_GenericFactory_ReplacesRuntimeTypeConstruction()
    {
        var source = AotFiles.GenericFactorySnippet();
        var build = BuildSnippets(new()
        {
            ["Factory.cs"] = source,
            ["Check.cs"] = """
                public static class Check
                {
                    public static int Run() => Factory.Create<int>().Count + Factory.NewArray<string>(3).Length;
                }
                """,
        });

        Assert.True(build.ExitCode == 0, build.Output);
        Assert.Empty(build.Diagnostics);
        Assert.DoesNotContain("MakeGenericType", source);
        Assert.DoesNotContain("CreateInstance", source);
    }

    [Fact]
    public void Todo05_RequiresDynamicCode_MovesTheWarningToTheCallers()
    {
        var source = AotFiles.DynamicCodeWrapperSnippet();
        var build = BuildSnippets(new()
        {
            ["DynamicFactory.cs"] = source,
            ["Use.cs"] = """
                using System;
                using System.Diagnostics.CodeAnalysis;

                public static class Use
                {
                    public static object Unannotated() => DynamicFactory.Create(typeof(int));

                    [RequiresDynamicCode("Callers must declare it too.")]
                    public static object Annotated() => DynamicFactory.Create(typeof(int));
                }
                """,
        });

        Assert.Contains("MakeGenericType", source);
        Assert.Contains("RequiresDynamicCode", source);
        // Inside the annotated method the dynamic code is the method's declared problem.
        Assert.Empty(build.In("DynamicFactory.cs"));
        // Only the caller that did not pass the annotation on is warned (line 6 of Use.cs).
        var warning = Assert.Single(build.In("Use.cs"));
        Assert.Equal("IL3050", warning.Id);
        Assert.Equal(6, warning.Line);
        Assert.Contains("DynamicFactory.Create", warning.Message);
    }

    [Fact]
    public void Todo06_StrictAotLibrary_FailsTheBuildOnAotWarnings()
    {
        var project = AotFiles.StrictAotLibrary();
        var props = _box.Evaluate(project, ["IsAotCompatible", "WarningsAsErrors", "PublishAot"]);
        Assert.Equal("true", props["IsAotCompatible"]);
        Assert.Contains("IL3050", props["WarningsAsErrors"]);
        Assert.Contains("IL2026", props["WarningsAsErrors"]);

        var bad = _box.Build(project, new() { ["ListMaker.cs"] = Snippets.MakeGenericType });
        Assert.NotEqual(0, bad.ExitCode);
        Assert.Contains("error IL3050", bad.Output);

        var good = _box.Build(project, new() { ["Clean.cs"] = Snippets.Clean });
        Assert.True(good.ExitCode == 0, good.Output);
    }

    [Fact]
    public void Todo07_GuardedFactory_UsesTheRuntimeFeatureCheck()
    {
        var source = AotFiles.GuardedFactorySnippet();
        var build = BuildSnippets(new()
        {
            ["SlowFactory.cs"] = Snippets.SlowFactory,
            ["SafeFactory.cs"] = source,
        });

        Assert.Contains("IsDynamicCodeSupported", source);
        Assert.Contains("SlowFactory.Create", source);
        Assert.True(build.ExitCode == 0, build.Output);
        Assert.Empty(build.Diagnostics);
    }
}
