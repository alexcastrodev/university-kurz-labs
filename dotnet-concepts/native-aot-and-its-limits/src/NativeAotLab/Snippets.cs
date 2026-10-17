namespace NativeAotLab;

/// <summary>
/// Ready-made C# files used by the lab (nothing to implement here). Each constant is a complete,
/// compilable source file. Several of them are deliberately AOT-unfriendly.
/// </summary>
public static class Snippets
{
    /// <summary>Builds a List of a type that is only known at run time.</summary>
    public const string MakeGenericType = """
        using System;
        using System.Collections.Generic;

        public static class ListMaker
        {
            public static object Create(Type item) =>
                Activator.CreateInstance(typeof(List<>).MakeGenericType(item))!;
        }
        """;

    /// <summary>Loads an assembly from a path chosen at run time.</summary>
    public const string LoadFrom = """
        using System.Reflection;

        public static class PluginLoader
        {
            public static Assembly Load(string path) => Assembly.LoadFrom(path);
        }
        """;

    /// <summary>Serializes with the reflection-based System.Text.Json overload.</summary>
    public const string JsonReflection = """
        using System.Text.Json;

        public record Note(string Text);

        public static class NoteJson
        {
            public static string ToJson(Note note) => JsonSerializer.Serialize(note);
        }
        """;

    /// <summary>A member that is annotated as needing dynamic code, used by the guard bonus.</summary>
    public const string SlowFactory = """
        using System;
        using System.Collections.Generic;
        using System.Diagnostics.CodeAnalysis;

        public static class SlowFactory
        {
            [RequiresDynamicCode("Builds a generic type at run time.")]
            public static object Create(Type item) =>
                Activator.CreateInstance(typeof(List<>).MakeGenericType(item))!;
        }
        """;

    /// <summary>A file with no AOT problem at all.</summary>
    public const string Clean = """
        public static class Clean
        {
            public static int Twice(int x) => x * 2;
        }
        """;
}
