namespace NativeAotLab;

/// <summary>
/// Each method returns text: a project file or a C# source file. The tests write the text to a
/// throwaway folder and either evaluate it with `dotnet msbuild -getProperty` (no restore, no
/// build) or build it with the AOT analyzers on. Nothing here ever publishes a native binary.
/// </summary>
public static class AotFiles
{
    // TODO-00: Return the .csproj of a console app that opts in to Native AOT: SDK
    //   Microsoft.NET.Sdk, TargetFramework net10.0, OutputType Exe and PublishAot true.
    //   The tests only EVALUATE this project. They never build or publish it, because that would
    //   download the ILCompiler runtime pack and need a native toolchain.
    public static string PublishAotProject() => throw new NotImplementedException();

    // TODO-01: Return the .csproj of a class library that declares itself AOT compatible:
    //   TargetFramework net10.0 and IsAotCompatible true. Do NOT set PublishAot: a library has
    //   nothing to publish, the flag only turns on the analyzers (and trimming metadata).
    public static string AotCompatibleLibrary() => throw new NotImplementedException();

    // TODO-02: Predict the analyzer. For each snippet in Snippets, return the warning IDs the
    //   AOT/trim analyzers report on it (IL2026 = RequiresUnreferencedCode, IL3050 =
    //   RequiresDynamicCode). The test builds the three files and compares with the real output.
    public static string[] WarningIdsForMakeGenericType() => throw new NotImplementedException();

    public static string[] WarningIdsForLoadFrom() => throw new NotImplementedException();

    public static string[] WarningIdsForJsonReflection() => throw new NotImplementedException();

    // TODO-03: Return a C# file that does the same job as Snippets.JsonReflection but is AOT safe:
    //   a public record Todo(int Id, string Title); a partial class TodoJsonContext deriving from
    //   JsonSerializerContext with [JsonSerializable(typeof(Todo))]; and a public static class
    //   TodoJson with ToJson(Todo) and FromJson(string) that pass TodoJsonContext.Default.Todo.
    //   Everything in the global namespace. No IL2026 or IL3050 may be left.
    public static string JsonSourceGeneratedSnippet() => throw new NotImplementedException();

    // TODO-04: Return a C# file with a public static class Factory (global namespace) that replaces
    //   Snippets.MakeGenericType and Array.CreateInstance with generics the compiler can see:
    //   List<T> Create<T>() and T[] NewArray<T>(int length). No IL3050 may be reported.
    public static string GenericFactorySnippet() => throw new NotImplementedException();

    // TODO-05: Return a C# file with a public static class DynamicFactory (global namespace) and a
    //   method `object Create(Type item)` that still uses MakeGenericType, but is honest about it:
    //   mark it [RequiresDynamicCode("...")]. The body then reports nothing, and every caller that
    //   does not carry the attribute gets IL3050 instead. The test adds two callers to check it.
    public static string DynamicCodeWrapperSnippet() => throw new NotImplementedException();

    // TODO-06 (optional): Return the .csproj of a library with IsAotCompatible true that turns the
    //   two AOT warnings into build errors (WarningsAsErrors IL2026;IL3050). Do not set PublishAot.
    public static string StrictAotLibrary() => throw new NotImplementedException();

    // TODO-07 (optional): Return a C# file with a public static class SafeFactory (global namespace)
    //   and `object Create(Type item)` that calls SlowFactory.Create (see Snippets.SlowFactory) only
    //   when RuntimeFeature.IsDynamicCodeSupported is true, and otherwise returns a new
    //   List<object>. The analyzer understands that guard, so no IL3050 is reported.
    public static string GuardedFactorySnippet() => throw new NotImplementedException();
}
