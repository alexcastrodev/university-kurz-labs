using System.Reflection;
using System.Reflection.Emit;
using Microsoft.CodeAnalysis;

namespace RoslynIlAndTheJit.Tests;

public class RoslynIlAndTheJitTests
{
    private const string Calc = """
        public static class Calc
        {
            public static int Add(int a, int b) { return a + b; }
            public static int AddChecked(int a, int b) { return checked(a + b); }
            public static int Twice(int x) { int y = x * 2; return y; }
        }
        """;

    private static byte[] Image(string source = Calc, OptimizationLevel level = OptimizationLevel.Release, OutputKind kind = OutputKind.DynamicallyLinkedLibrary)
    {
        var result = CompilerPipeline.Compile(source, level, kind);
        Assert.True(result.Success, string.Join("; ", result.Diagnostics));
        return result.Image!;
    }

    private static IReadOnlyList<OpCode> Ops(byte[] image, string method) =>
        IlInspector.ReadOpCodes(IlInspector.GetMethodIl(image, "Calc", method));

    [Fact]
    public void ShouldCompileValidSourceToAnAssemblyImage()
    {
        var result = CompilerPipeline.Compile(Calc);

        Assert.True(result.Success);
        Assert.NotEmpty(result.Image!);
        Assert.DoesNotContain(result.Diagnostics, d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void ShouldReportCompileErrorsAsDiagnosticsWithoutAnImage()
    {
        var result = CompilerPipeline.Compile("class C { int M() { return \"text\"; } }");

        Assert.False(result.Success);
        Assert.Null(result.Image);
        Assert.Contains(result.Diagnostics, d => d.Id == "CS0029" && d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void ShouldRunACompiledMethodByReflection()
    {
        var image = Image();

        Assert.Equal(5, CompilerPipeline.Run(image, "Calc", "Add", 2, 3));
        Assert.Equal(14, CompilerPipeline.Run(image, "Calc", "Twice", 7));
    }

    [Fact]
    public void ShouldLetTheRuntimeExceptionSurfaceFromACheckedOverflow()
    {
        var image = Image();

        var ex = Assert.Throws<TargetInvocationException>(
            () => CompilerPipeline.Run(image, "Calc", "AddChecked", int.MaxValue, 1));
        Assert.IsType<OverflowException>(ex.InnerException);
        Assert.Equal(int.MinValue, CompilerPipeline.Run(image, "Calc", "Add", int.MaxValue, 1));
    }

    [Fact]
    public void ShouldReadTheOpcodesOfASimpleMethod()
    {
        var ops = Ops(Image(), "Add");

        Assert.Equal([OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Add, OpCodes.Ret], ops);
    }

    [Fact]
    public void ShouldEmitAddOvfOnlyInsideCheckedContext()
    {
        var image = Image();

        Assert.Contains(OpCodes.Add_Ovf, Ops(image, "AddChecked"));
        Assert.DoesNotContain(OpCodes.Add, Ops(image, "AddChecked"));
        Assert.Contains(OpCodes.Add, Ops(image, "Add"));
        Assert.DoesNotContain(OpCodes.Add_Ovf, Ops(image, "Add"));
    }

    [Fact]
    public void ShouldEmitNopsInDebugButNotInRelease()
    {
        var debug = Ops(Image(level: OptimizationLevel.Debug), "Twice");
        var release = Ops(Image(level: OptimizationLevel.Release), "Twice");

        Assert.Contains(OpCodes.Nop, debug);
        Assert.DoesNotContain(OpCodes.Nop, release);
        Assert.True(debug.Count > release.Count);
    }

    [Fact]
    public void ShouldWalkTwoByteOpcodesAndSkipOperands()
    {
        // ldc.i4 1000; ldc.i8 5; ceq (two-byte 0xFE 0x01); ret
        byte[] il = [0x20, 0xE8, 0x03, 0x00, 0x00, 0x21, 5, 0, 0, 0, 0, 0, 0, 0, 0xFE, 0x01, 0x2A];

        Assert.Equal([OpCodes.Ldc_I4, OpCodes.Ldc_I8, OpCodes.Ceq, OpCodes.Ret], IlInspector.ReadOpCodes(il));
    }

    [Fact]
    public void ShouldFailForAMethodThatDoesNotExist()
    {
        Assert.Throws<ArgumentException>(() => IlInspector.GetMethodIl(Image(), "Calc", "Missing"));
    }

    [Fact]
    public void ShouldListDeclaredMethodsFromTheSyntaxTree()
    {
        Assert.Equal(["Add", "AddChecked", "Twice"], SyntaxAndJit.DeclaredMethodNames(Calc));
        Assert.Empty(SyntaxAndJit.DeclaredMethodNames("class Empty { }"));
    }

    [Fact]
    public void ShouldAskTheSemanticModelForTheTypeOfAnExpression()
    {
        const string source = """
            class C
            {
                void M()
                {
                    var a = 1 + 2L;
                    var b = "x" + 1;
                    var c = 1 + 2;
                }
            }
            """;

        Assert.Equal("long", SyntaxAndJit.TypeOfInitializer(source, "a"));
        Assert.Equal("string", SyntaxAndJit.TypeOfInitializer(source, "b"));
        Assert.Equal("int", SyntaxAndJit.TypeOfInitializer(source, "c"));
    }

    [Fact]
    public void ShouldReadJitHintsFromMetadata()
    {
        Assert.True(SyntaxAndJit.GetImplFlags(typeof(Hints), nameof(Hints.Cold)).HasFlag(MethodImplAttributes.NoInlining));
        Assert.True(SyntaxAndJit.GetImplFlags(typeof(Hints), nameof(Hints.Hot)).HasFlag(MethodImplAttributes.AggressiveInlining));
        var plain = SyntaxAndJit.GetImplFlags(typeof(Hints), nameof(Hints.Plain));
        Assert.False(plain.HasFlag(MethodImplAttributes.NoInlining));
        Assert.False(plain.HasFlag(MethodImplAttributes.AggressiveInlining));
    }

    [Fact]
    public void Bonus_ShouldFindTheCompilerGeneratedEntryPointOfTopLevelStatements()
    {
        var exe = Image("System.Console.WriteLine(1);", kind: OutputKind.ConsoleApplication);

        Assert.Equal("<Main>$", IlInspector.GetEntryPointName(exe));
        Assert.Null(IlInspector.GetEntryPointName(Image()));
    }

    [Fact]
    public void Bonus_ShouldPrepareAMethodWithoutChangingItsResult()
    {
        var method = typeof(Hints).GetMethod(nameof(Hints.Cold))!;

        SyntaxAndJit.ForceJit(method);

        Assert.Equal(2, Hints.Cold(1));
    }
}
