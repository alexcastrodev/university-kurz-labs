using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynIlAndTheJit;

public static class SyntaxAndJit
{
    public static IReadOnlyList<string> DeclaredMethodNames(string source)
    {
        // TODO-04: Parse the source and return the identifier of every MethodDeclarationSyntax,
        //   in source order (DescendantNodes().OfType<...>()). No compilation needed for this one.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static string TypeOfInitializer(string source, string variableName)
    {
        // TODO-04 (continued): Compile the source (syntax tree + CSharpCompilation with CompilerPipeline.References),
        //   find the VariableDeclaratorSyntax named `variableName`, ask compilation.GetSemanticModel(tree)
        //   for GetTypeInfo(declarator.Initializer!.Value).Type and return its ToDisplayString().
        //   The syntax says `1 + 2L`; only the semantic model knows that this is a long.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static MethodImplAttributes GetImplFlags(Type type, string methodName)
    {
        // TODO-05: Return the MethodImplAttributes of the method (GetMethodImplementationFlags).
        //   [MethodImpl(NoInlining)] and [MethodImpl(AggressiveInlining)] are recorded in metadata:
        //   they are hints to the JIT, readable without running anything.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static void ForceJit(MethodInfo method)
    {
        // TODO-07 (optional): Ask the runtime to JIT the method now, before its first call
        //   (RuntimeHelpers.PrepareMethod with method.MethodHandle).
        throw new NotImplementedException("Not implemented yet.");
    }
}

public static class Hints
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static int Cold(int x) => x + 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Hot(int x) => x + 1;

    public static int Plain(int x) => x + 1;
}
