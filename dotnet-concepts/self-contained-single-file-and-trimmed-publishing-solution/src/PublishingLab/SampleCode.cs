namespace PublishingLab;

/// <summary>Code given to you. It is correct C#, but it is not safe for trimming or single-file apps.</summary>
public static class SampleCode
{
    /// <summary>
    /// A small program with four classic problems: a type chosen from a string (IL2057), reflection
    /// over an unannotated Type parameter (IL2070), reflection-based JSON (IL2026) and
    /// Assembly.Location (IL3000).
    /// </summary>
    public const string RiskyProgram = """
        using System.Reflection;
        using System.Text.Json;

        var name = args.Length > 0 ? args[0] : "Plugin";
        Console.WriteLine($"type={Type.GetType(name)?.Name}");
        Console.WriteLine($"methods={string.Join(",", MethodNames(typeof(Invoice)))}");
        Console.WriteLine($"json={JsonSerializer.Serialize(new Invoice())}");
        Console.WriteLine($"base={Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}");

        static IEnumerable<string> MethodNames(Type t) =>
            t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(m => m.Name)
                .Order(StringComparer.Ordinal);

        class Plugin { }

        public class Invoice
        {
            public decimal Total { get; set; } = 10m;
            public void Print() { }
        }
        """;
}
