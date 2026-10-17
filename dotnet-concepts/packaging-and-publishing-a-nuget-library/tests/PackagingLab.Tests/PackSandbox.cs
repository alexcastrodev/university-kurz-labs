using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace PackagingLab.Tests;

/// <summary>A throwaway folder tree plus helpers that run the .NET CLI and open the produced packages.</summary>
public sealed class PackSandbox : IDisposable
{
    public string Root { get; } = Path.Combine(Path.GetTempPath(), "packaginglab-" + Guid.NewGuid().ToString("N"));

    public string OutDir => Path.Combine(Root, "out");

    public PackSandbox() => Directory.CreateDirectory(Root);

    public string Write(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
        return path;
    }

    /// <summary>`dotnet pack -c Release -o out` for one project. Offline: the projects have no PackageReference.</summary>
    public void Pack(string project, params string[] extra) =>
        Run(["pack", project, "-c", "Release", "-o", OutDir, .. extra]);

    public string[] OutFiles() =>
        Directory.Exists(OutDir)
            ? Directory.GetFiles(OutDir).Select(Path.GetFileName).Select(n => n!).Order(StringComparer.Ordinal).ToArray()
            : [];

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
        // The timeout is only a safety net, never an assertion.
        if (!process.WaitForExit(TimeSpan.FromSeconds(180)))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException("dotnet " + string.Join(' ', args) + " did not finish in 180 s");
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

/// <summary>A read-only view of a package (zip) with the parsed manifest.</summary>
public sealed class Package
{
    public string[] Entries { get; }
    public XDocument Nuspec { get; }

    public Package(string path)
    {
        using var zip = ZipFile.OpenRead(path);
        Entries = zip.Entries.Select(e => e.FullName.Replace('\\', '/')).Order(StringComparer.Ordinal).ToArray();
        var nuspec = zip.Entries.Single(e => e.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
        using var stream = nuspec.Open();
        Nuspec = XDocument.Load(stream);
    }

    private XElement? Find(string name) =>
        Nuspec.Descendants().FirstOrDefault(e => e.Name.LocalName == name);

    /// <summary>Text of a metadata element such as id, version or tags, or "" when it is missing.</summary>
    public string Meta(string name) => Find(name)?.Value ?? "";

    /// <summary>The value of an attribute on a metadata element, or "" when missing.</summary>
    public string Attr(string element, string attribute) =>
        Find(element)?.Attribute(attribute)?.Value ?? "";

    /// <summary>All dependency ids declared in the manifest, as "id version".</summary>
    public string[] Dependencies() =>
        Nuspec.Descendants().Where(e => e.Name.LocalName == "dependency")
            .Select(e => $"{e.Attribute("id")!.Value} {e.Attribute("version")!.Value}")
            .Distinct().Order(StringComparer.Ordinal).ToArray();
}
