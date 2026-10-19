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
        return CSharpSyntaxTree.ParseText(source).GetRoot()
            .DescendantNodes().OfType<MethodDeclarationSyntax>()
            .Select(m => m.Identifier.Text)
            .ToList();
    }

    public static string TypeOfInitializer(string source, string variableName)
    {
        // TODO-04 (continued): Compile the source (syntax tree + CSharpCompilation with CompilerPipeline.References),
        //   find the VariableDeclaratorSyntax named `variableName`, ask compilation.GetSemanticModel(tree)
        //   for GetTypeInfo(declarator.Initializer!.Value).Type and return its ToDisplayString().
        //   The syntax says `1 + 2L`; only the semantic model knows that this is a long.
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("Lab", [tree], CompilerPipeline.References);
        var declarator = tree.GetRoot().DescendantNodes().OfType<VariableDeclaratorSyntax>()
            .First(d => d.Identifier.Text == variableName);
        var type = compilation.GetSemanticModel(tree).GetTypeInfo(declarator.Initializer!.Value).Type!;
        return type.ToDisplayString();
    }

    public static MethodImplAttributes GetImplFlags(Type type, string methodName)
    {
        // TODO-05: Return the MethodImplAttributes of the method (GetMethodImplementationFlags).
        //   [MethodImpl(NoInlining)] and [MethodImpl(AggressiveInlining)] are recorded in metadata:
        //   they are hints to the JIT, readable without running anything.
        return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)!
            .GetMethodImplementationFlags();
    }

    public static void ForceJit(MethodInfo method)
    {
        // TODO-07 (optional): Ask the runtime to JIT the method now, before its first call
        //   (RuntimeHelpers.PrepareMethod with method.MethodHandle).
        RuntimeHelpers.PrepareMethod(method.MethodHandle);
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
