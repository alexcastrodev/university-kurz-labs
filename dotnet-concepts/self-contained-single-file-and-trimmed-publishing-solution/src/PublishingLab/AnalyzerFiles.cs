namespace PublishingLab;

/// <summary>
/// The analyzer half of the lab. The tests build a plain framework-dependent project (no
/// RuntimeIdentifier, so nothing is downloaded) and read the IL warnings from the build output.
/// </summary>
public static class AnalyzerFiles
{
    // TODO-04: Return a framework-dependent console project (net10.0, ImplicitUsings and Nullable enabled)
    //          on which the compiler runs BOTH the trim analyzer and the single-file analyzer, so that
    //          building SampleCode.RiskyProgram reports IL2026, IL2057, IL2070 and IL3000.
    //          Hint: PublishTrimmed and PublishSingleFile switch the analyzers on, or set
    //          EnableTrimAnalyzer and EnableSingleFileAnalyzer directly.
    public static string AnalyzerProject() =>
        """
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
            <EnableSingleFileAnalyzer>true</EnableSingleFileAnalyzer>
          </PropertyGroup>

        </Project>
        """;

    // TODO-05: Return the RiskyProgram rewritten so the analyzers report NO IL warning and it prints the
    //          same four lines (type=, methods=, json=, base=). Fixes to apply:
    //            - Type.GetType(name)          -> typeof(Plugin)
    //            - Type parameter t            -> [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    //            - JsonSerializer.Serialize(x) -> a source-generated JsonSerializerContext
    //            - Assembly.Location           -> AppContext.BaseDirectory
    public static string FixedProgram() =>
        """
        using System.Diagnostics.CodeAnalysis;
        using System.Reflection;
        using System.Text.Json;
        using System.Text.Json.Serialization;

        Console.WriteLine($"type={typeof(Plugin).Name}");
        Console.WriteLine($"methods={string.Join(",", MethodNames(typeof(Invoice)))}");
        Console.WriteLine($"json={JsonSerializer.Serialize(new Invoice(), AppJsonContext.Default.Invoice)}");
        Console.WriteLine($"base={Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory)}");

        static IEnumerable<string> MethodNames(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type t) =>
            t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name)
                .Order(StringComparer.Ordinal);

        class Plugin { }

        public class Invoice
        {
            public decimal Total { get; set; } = 10m;
            public void Print() { }
        }

        [JsonSerializable(typeof(Invoice))]
        partial class AppJsonContext : JsonSerializerContext { }
        """;

    // TODO-07 (optional): Return the same project as AnalyzerProject, plus WarningsAsErrors for
    //          IL2026, IL2057 and IL2070, so building the risky program FAILS instead of warning.
    public static string StrictAnalyzerProject() =>
        """
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <ImplicitUsings>enable</ImplicitUsings>
            <Nullable>enable</Nullable>
            <PublishTrimmed>true</PublishTrimmed>
            <PublishSingleFile>true</PublishSingleFile>
            <WarningsAsErrors>$(WarningsAsErrors);IL2026;IL2057;IL2070</WarningsAsErrors>
          </PropertyGroup>

        </Project>
        """;
}
