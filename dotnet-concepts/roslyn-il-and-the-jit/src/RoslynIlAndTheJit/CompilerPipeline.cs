using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynIlAndTheJit;

/// <summary>The outcome of an in-memory compilation. <see cref="Image"/> is null when there were errors.</summary>
public sealed record CompileResult(byte[]? Image, ImmutableArray<Diagnostic> Diagnostics)
{
    public bool Success => Image is not null;
}

public static class CompilerPipeline
{
    // Every assembly the running process trusts, as file paths. Handy as Roslyn references.
    public static readonly IReadOnlyList<MetadataReference> References =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

    public static CompileResult Compile(
        string source,
        OptimizationLevel optimization = OptimizationLevel.Release,
        OutputKind kind = OutputKind.DynamicallyLinkedLibrary)
    {
        // TODO-00: Parse `source`, create a CSharpCompilation named "Lab" with `References` and
        //   CSharpCompilationOptions(kind, optimizationLevel: optimization), and Emit it to a MemoryStream.
        //   Return the bytes on success (null on failure) together with the emit diagnostics.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static object? Run(byte[] image, string typeName, string methodName, params object?[] args)
    {
        // TODO-01: Load the image into a collectible AssemblyLoadContext, find the public static method by
        //   type and method name with reflection, invoke it and return the result.
        //   Always Unload() the context, even when the call throws. Do not unwrap TargetInvocationException.
        throw new NotImplementedException("Not implemented yet.");
    }
}
