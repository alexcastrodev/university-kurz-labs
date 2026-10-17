using System.Diagnostics;
using System.Text.Json;

namespace PublishingLab.Tests;

/// <summary>
/// A throwaway project folder plus helpers that run the .NET CLI. It only evaluates (`dotnet msbuild -getProperty`)
/// and builds a framework-dependent project. It never publishes and never restores a RuntimeIdentifier.
/// </summary>
public sealed class PublishSandbox : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "publishinglab-" + Guid.NewGuid().ToString("N"));

    public PublishSandbox() => Directory.CreateDirectory(Root);

    public string Write(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>Evaluates a project (no restore, no build) and returns the parsed JSON.</summary>
    public JsonElement Evaluate(string csproj, string[] properties, string[] items, params string[] extra)
    {
        var args = new List<string> { "msbuild", Write("App.csproj", csproj) };
        // A single -getProperty prints the bare value, so always ask for a second one to get JSON.
        args.Add("-getProperty:" + string.Join(',', properties.Append("MSBuildProjectName")));
        if (items.Length > 0) args.Add("-getItem:" + string.Join(',', items));
        args.AddRange(extra);
        var (code, output) = Run(args);
        if (code != 0) throw new InvalidOperationException("dotnet msbuild failed:\n" + output);
        return JsonDocument.Parse(output).RootElement.Clone();
    }

    public static string Prop(JsonElement result, string name) =>
        result.GetProperty("Properties").GetProperty(name).GetString() ?? "";

    public static string[] Identities(JsonElement result, string item) =>
        result.GetProperty("Items").GetProperty(item).EnumerateArray()
            .Select(i => i.GetProperty("Identity").GetString()!)
            .ToArray();

    /// <summary>Builds a framework-dependent project (so the analyzers run) and returns the exit code and output.</summary>
    public (int ExitCode, string Output) Build(string csproj, string program)
    {
        Write("App.csproj", csproj);
        Write("Program.cs", program);
        return Run(["build", "App.csproj", "--nologo", "-v:minimal"]);
    }

    public (int ExitCode, string Output) RunBuiltApp()
    {
        var dll = Directory.GetFiles(Path.Combine(Root, "bin"), "App.dll", SearchOption.AllDirectories).Single();
        return Run([dll]);
    }

    private (int ExitCode, string Output) Run(IEnumerable<string> args)
    {
        var psi = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Root,
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
        // Safety net only, never an assertion about speed.
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
