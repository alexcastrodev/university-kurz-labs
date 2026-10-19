using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NativeAotLab.Tests;

/// <summary>One analyzer warning or error from a build: file name, line and diagnostic id.</summary>
public sealed record Diagnostic(string File, int Line, string Id, string Message);

public sealed record BuildResult(int ExitCode, string Output, IReadOnlyList<Diagnostic> Diagnostics, string Dll)
{
    public IEnumerable<Diagnostic> In(string file) => Diagnostics.Where(d => d.File == file);
}

/// <summary>
/// A throwaway folder plus helpers that run the .NET CLI. It only ever evaluates projects or builds
/// framework-dependent ones. It never passes PublishAot to a build or publish, so nothing is
/// downloaded and no native toolchain is needed.
/// </summary>
public sealed class AotSandbox : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "nativeaotlab-" + Guid.NewGuid().ToString("N"));

    /// <summary>Fixed project used when the student does not provide one (analyzers on, no PublishAot).</summary>
    public const string AnalyzerProject = """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <IsAotCompatible>true</IsAotCompatible>
          </PropertyGroup>
        </Project>
        """;

    public AotSandbox() => Directory.CreateDirectory(Root);

    public string Write(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>Evaluates a project (no restore, no build) and returns the properties by name.</summary>
    public Dictionary<string, string> Evaluate(string projectText, string[] properties, params string[] extra)
    {
        var project = Write("eval/Eval.csproj", projectText);
        var args = new List<string> { "msbuild", project };
        // A single -getProperty prints the bare value, so always ask for a second one to get JSON.
        args.Add("-getProperty:" + string.Join(',', properties.Append("MSBuildProjectName")));
        args.AddRange(extra);
        var run = Run(args);
        if (run.ExitCode != 0) throw new InvalidOperationException(run.Output);
        using var doc = JsonDocument.Parse(run.Output);
        return doc.RootElement.GetProperty("Properties").EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value.GetString() ?? "");
    }

    /// <summary>
    /// Builds a library from the given project text and source files. Only framework-dependent
    /// builds: guarded so that a project with PublishAot=true is never built.
    /// </summary>
    public BuildResult Build(string projectText, Dictionary<string, string> sources)
    {
        var dir = "b" + Guid.NewGuid().ToString("N")[..8];
        var project = Write($"{dir}/Lib.csproj", projectText);
        foreach (var (name, text) in sources) Write($"{dir}/{name}", text);

        var guard = Evaluate(projectText, ["PublishAot"]);
        if (guard["PublishAot"] != "")
            throw new InvalidOperationException("The lab never builds a project that sets PublishAot: evaluate it instead.");

        var run = Run(["build", project, "--no-incremental", "-nologo", "-nodeReuse:false",
            "-p:UseSharedCompilation=false", "-o", Path.Combine(Root, dir, "out")]);
        return new BuildResult(run.ExitCode, run.Output, ParseDiagnostics(run.Output), Path.Combine(Root, dir, "out", "Lib.dll"));
    }

    private static readonly Regex DiagnosticLine =
        new(@"(?<file>[^/\\\s]+\.cs)\((?<line>\d+),\d+\): (?:warning|error) (?<id>IL\d+): (?<msg>[^\[]*)", RegexOptions.Compiled);

    /// <summary>The build prints each diagnostic twice (once live, once in the summary): keep one.</summary>
    public static IReadOnlyList<Diagnostic> ParseDiagnostics(string output) =>
        DiagnosticLine.Matches(output)
            .Select(m => new Diagnostic(m.Groups["file"].Value, int.Parse(m.Groups["line"].Value),
                m.Groups["id"].Value, m.Groups["msg"].Value.Trim()))
            .DistinctBy(d => (d.File, d.Line, d.Id))
            .ToList();

    private static (int ExitCode, string Output) Run(IEnumerable<string> args)
    {
        var psi = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var a in args) psi.ArgumentList.Add(a);

        // Do not inherit state from the outer `dotnet test` process.
        foreach (var key in psi.Environment.Keys.ToList())
        {
            if (key.StartsWith("MSBUILD", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("DOTNET_HOST_PATH", StringComparison.OrdinalIgnoreCase)
                || key.StartsWith("VSTEST", StringComparison.OrdinalIgnoreCase))
                psi.Environment.Remove(key);
        }
        psi.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        psi.Environment["DOTNET_NOLOGO"] = "1";
        psi.Environment["DOTNET_SKIP_FIRST_TIME_EXPERIENCE"] = "1";

        using var process = Process.Start(psi)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        // Safety net only: a normal run takes a few seconds.
        if (!process.WaitForExit(TimeSpan.FromSeconds(180)))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("dotnet " + string.Join(' ', args) + " did not finish in 180 s");
        }
        process.WaitForExit();
        return (process.ExitCode, stdout.Result + stderr.Result);
    }

    public void Dispose()
    {
        try { Directory.Delete(Root, recursive: true); } catch (IOException) { }
    }
}
