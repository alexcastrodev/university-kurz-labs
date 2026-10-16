using System.Diagnostics.CodeAnalysis;

namespace SourceGeneratorsAndAotFriendlyCode;

public static class ReflectionBaseline
{
    // The runtime-reflection way to do what the generator does at compile time.
    // The attribute is how the trim and AOT analyzers (IL2026) learn that callers are not safe.
    [RequiresUnreferencedCode("Reads public properties by reflection, which the trimmer cannot see.")]
    public static string ToDisplayString(object value)
    {
        var type = value.GetType();
        var properties = type.GetProperties()
            .Where(p => p.GetMethod is { IsPublic: true, IsStatic: false } && p.GetIndexParameters().Length == 0)
            .Select(p => $"{p.Name}={p.GetValue(value)}")
            .ToList();
        return properties.Count == 0 ? $"{type.Name} {{ }}" : $"{type.Name} {{ {string.Join(", ", properties)} }}";
    }
}
