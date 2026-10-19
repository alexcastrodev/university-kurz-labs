using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FileBasedAppsLab.Tests;

public sealed class FileBasedAppTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    public FileBasedAppTests() => Directory.CreateDirectory(_dir);

    public void Dispose()
    {
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    private record Result(int ExitCode, string Output);

    // Runs "dotnet <dotnetArgs>" inside the temp directory, with the outer test host state removed.
    private Result Dotnet(params string[] dotnetArgs)
    {
        var psi = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = _dir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var a in dotnetArgs) psi.ArgumentList.Add(a);
        foreach (var key in psi.Environment.Keys.ToList())
        {
            if (key.StartsWith("MSBUILD", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("DOTNET_HOST_PATH", StringComparison.OrdinalIgnoreCase))
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
            throw new TimeoutException("dotnet did not finish in 180 seconds.");
        }
        process.WaitForExit();
        return new Result(process.ExitCode, stdout.Result + stderr.Result);
    }

    private Result Run(string source, params string[] appArgs)
    {
        File.WriteAllText(Path.Combine(_dir, "app.cs"), source);
        var args = new List<string> { "run", "app.cs" };
        if (appArgs.Length > 0) { args.Add("--"); args.AddRange(appArgs); }
        return Dotnet(args.ToArray());
    }

    // A fixed program (not part of the TODOs) that reports whether dynamic code is available.
    private const string DynamicCodeProbe =
        "Console.WriteLine(System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported);";

    [Fact]
    public void ShouldRunASingleFileWithoutAProjectFile()
    {
        var result = Run(Apps.HelloApp());

        Assert.Equal(0, result.ExitCode);
        Assert.Equal("hello from a file-based app", result.Output.Trim());
    }

    [Fact]
    public void ShouldPassArgumentsAfterTheDoubleDashAndReturnTheExitCode()
    {
        var result = Run(Apps.AppWithArgs(), "one", "two", "three");

        Assert.Equal(3, result.ExitCode);
        Assert.Equal("one,two,three", result.Output.Trim());
    }

    [Fact]
    public void ShouldTurnOffTheAotDefaultWithAPropertyDirective()
    {
        var result = Run(Apps.AppWithProperty());

        Assert.Equal(0, result.ExitCode);
        Assert.Equal("True", result.Output.Trim());
    }

    [Fact]
    public void ShouldKeepTheAotDefaultWhenNoPropertyIsSet()
    {
        // The same probe without the directive: the generated project targets Native AOT.
        var result = Run(DynamicCodeProbe);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal("False", result.Output.Trim());
    }

    [Fact]
    public void ShouldAcceptAShebangAsTheFirstLine()
    {
        if (OperatingSystem.IsWindows()) return; // shebangs are a Unix feature

        var source = Apps.ShebangApp();
        Assert.StartsWith("#!/usr/bin/env dotnet", source);

        var result = Run(source);

        Assert.Equal(0, result.ExitCode);
        Assert.Equal("shebang ok", result.Output.Trim());
    }

    [Fact]
    public void ShouldReportACompileErrorWithAnIdAndANonZeroExitCode()
    {
        var result = Run(Apps.AppWithCompileError());

        Assert.NotEqual(0, result.ExitCode);
        Assert.Matches(new Regex(@"error CS\d{4}"), result.Output);
    }

    // Bonus
    [Fact]
    public void ShouldShowThePropertyInTheConvertedProject()
    {
        File.WriteAllText(Path.Combine(_dir, "app.cs"), Apps.AppWithProperty());

        var result = Dotnet("project", "convert", "app.cs");

        Assert.Equal(0, result.ExitCode);
        var csproj = File.ReadAllText(Path.Combine(_dir, "app", "app.csproj"));
        Assert.Contains("<PublishAot>false</PublishAot>", csproj);
        Assert.DoesNotContain("#:property", File.ReadAllText(Path.Combine(_dir, "app", "app.cs")));
    }
}
