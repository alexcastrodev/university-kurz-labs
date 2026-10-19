using System.Diagnostics;
using System.Text.Json;

namespace SdkStyleLab.Tests;

/// <summary>A throwaway directory tree plus a helper that asks `dotnet msbuild` what it evaluated.</summary>
public sealed class MsBuildSandbox : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "sdkstylelab-" + Guid.NewGuid().ToString("N"));

    public MsBuildSandbox() => Directory.CreateDirectory(Root);

    public string Write(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>Evaluates a project (no restore, no build) and returns the parsed JSON.</summary>
    public JsonElement Evaluate(string project, string[] properties, string[] items, params string[] extra)
    {
        var args = new List<string> { "msbuild", project };
        // A single -getProperty prints the bare value, so always ask for a second one to get JSON.
        if (properties.Length > 0) args.Add("-getProperty:" + string.Join(',', properties.Append("MSBuildProjectName")));
        if (items.Length > 0) args.Add("-getItem:" + string.Join(',', items));
        args.AddRange(extra);
        return JsonDocument.Parse(Run(args)).RootElement.Clone();
    }

    public static string Prop(JsonElement result, string name) =>
        result.GetProperty("Properties").GetProperty(name).GetString() ?? "";

    /// <summary>Compile items as paths relative to the project folder, with forward slashes.</summary>
    public static string[] CompileItems(JsonElement result) =>
        result.GetProperty("Items").GetProperty("Compile").EnumerateArray()
            .Select(i => i.GetProperty("Identity").GetString()!.Replace('\\', '/'))
            .Order(StringComparer.Ordinal)
            .ToArray();

    public string Run(IEnumerable<string> args)
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
        if (!process.WaitForExit(TimeSpan.FromSeconds(120)))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("dotnet " + string.Join(' ', args) + " did not finish in 120 s");
        }
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"dotnet {string.Join(' ', args)} failed:\n{stdout.Result}\n{stderr.Result}");
        return stdout.Result;
    }

    public void Dispose()
    {
        try { Directory.Delete(Root, recursive: true); } catch (IOException) { }
    }
}
