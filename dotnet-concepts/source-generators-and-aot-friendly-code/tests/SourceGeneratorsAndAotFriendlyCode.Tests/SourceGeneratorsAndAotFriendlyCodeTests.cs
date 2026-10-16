using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SourceGeneratorsAndAotFriendlyCode.Tests;

public class SourceGeneratorsAndAotFriendlyCodeTests
{
    private const string Person = """
        using Gen;
        namespace Demo;

        [AutoToString]
        public partial class Person
        {
            public string Name { get; set; } = "";
            public int Age { get; set; }
            private int Secret { get; set; }
            public static int Counter { get; set; }
        }
        """;

    private const string Unmarked = """
        namespace Demo;
        public class Plain { public int X { get; set; } }
        """;

    private static readonly IReadOnlyList<MetadataReference> References =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

    private static CSharpCompilation Compile(params SyntaxTree[] trees) =>
        CSharpCompilation.Create("Lab", trees, References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static SyntaxTree Tree(string source, string path = "Input.cs") =>
        CSharpSyntaxTree.ParseText(source, path: path);

    private static GeneratorDriver NewDriver(bool tracking = false) =>
        CSharpGeneratorDriver.Create(
            generators: [new AutoToStringGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: tracking));

    private static GeneratorRunResult Generate(params string[] sources)
    {
        var compilation = Compile(sources.Select((s, i) => Tree(s, $"Input{i}.cs")).ToArray());
        return NewDriver().RunGenerators(compilation).GetRunResult().Results[0];
    }

    private static string GeneratedText(GeneratorRunResult result, string hintName) =>
        result.GeneratedSources.Single(s => s.HintName == hintName).SourceText.ToString();

    private static Assembly Load(Compilation output)
    {
        using var stream = new MemoryStream();
        var emit = output.Emit(stream);
        Assert.True(emit.Success, string.Join("; ", emit.Diagnostics));
        return Assembly.Load(stream.ToArray());
    }

    [Fact]
    public void ShouldAddTheMarkerAttributeBeforeTheUserCodeCompiles()
    {
        var result = Generate(Person);

        Assert.Contains("AutoToString", GeneratedText(result, "AutoToStringAttribute.g.cs"));
    }

    [Fact]
    public void ShouldGenerateOnlyForClassesMarkedWithTheAttribute()
    {
        var result = Generate(Person, Unmarked);

        Assert.Contains(result.GeneratedSources, s => s.HintName == "Person.g.cs");
        Assert.DoesNotContain(result.GeneratedSources, s => s.HintName == "Plain.g.cs");
    }

    [Fact]
    public void ShouldKeepAnAttributeWithTheSameNameInAnotherNamespaceIgnored()
    {
        // ForAttributeWithMetadataName matches the full metadata name, not the simple name.
        var impostor = """
            namespace Other { public class AutoToStringAttribute : System.Attribute { } }
            namespace Demo { [Other.AutoToString] public partial class Fake { public int X { get; set; } } }
            """;

        var result = Generate(impostor);

        Assert.Contains(result.GeneratedSources, s => s.HintName == "AutoToStringAttribute.g.cs");
        Assert.DoesNotContain(result.GeneratedSources, s => s.HintName == "Fake.g.cs");
    }

    [Fact]
    public void ShouldListOnlyPublicInstanceProperties()
    {
        var text = GeneratedText(Generate(Person), "Person.g.cs");

        Assert.Contains("namespace Demo", text);
        Assert.Contains("partial class Person", text);
        Assert.Contains("ToDisplayString", text);
        Assert.Contains("Name", text);
        Assert.Contains("Age", text);
        Assert.DoesNotContain("Secret", text);
        Assert.DoesNotContain("Counter", text);
    }

    [Fact]
    public void ShouldProduceAnOutputCompilationWithoutErrors()
    {
        var compilation = Compile(Tree(Person));
        var driver = NewDriver().RunGeneratorsAndUpdateCompilation(compilation, out var output, out var diagnostics);

        Assert.Empty(diagnostics);
        Assert.DoesNotContain(output.GetDiagnostics(), d => d.Severity == DiagnosticSeverity.Error);
    }

    [Fact]
    public void ShouldGenerateAMethodThatReallyWorks()
    {
        NewDriver().RunGeneratorsAndUpdateCompilation(Compile(Tree(Person)), out var output, out _);
        var assembly = Load(output);
        var type = assembly.GetType("Demo.Person")!;
        var person = Activator.CreateInstance(type)!;
        type.GetProperty("Name")!.SetValue(person, "Ada");
        type.GetProperty("Age")!.SetValue(person, 36);

        var text = (string)type.GetMethod("ToDisplayString")!.Invoke(person, null)!;

        Assert.Equal("Person { Name=Ada, Age=36 }", text);
    }

    [Fact]
    public void ShouldSupportClassesInTheGlobalNamespace()
    {
        var source = "using Gen; [AutoToString] public partial class Point { public int X { get; set; } }";
        NewDriver().RunGeneratorsAndUpdateCompilation(Compile(Tree(source)), out var output, out _);

        Assert.DoesNotContain(output.GetDiagnostics(), d => d.Severity == DiagnosticSeverity.Error);
        Assert.NotNull(Load(output).GetType("Point")!.GetMethod("ToDisplayString"));
    }

    [Fact]
    public void ShouldGenerateCodeWithoutAnyReflection()
    {
        var text = GeneratedText(Generate(Person), "Person.g.cs");

        Assert.DoesNotContain("GetProperties", text);
        Assert.DoesNotContain("GetProperty", text);
        Assert.DoesNotContain("Activator", text);
        Assert.DoesNotContain("GetMethod", text);
        Assert.DoesNotContain("System.Reflection", text);
        Assert.DoesNotContain("typeof", text);
    }

    [Fact]
    public void ShouldGiveTheSameTextAsTheReflectionBaseline()
    {
        NewDriver().RunGeneratorsAndUpdateCompilation(Compile(Tree(Person)), out var output, out _);
        var type = Load(output).GetType("Demo.Person")!;
        var person = Activator.CreateInstance(type)!;
        type.GetProperty("Name")!.SetValue(person, "Grace");
        type.GetProperty("Age")!.SetValue(person, 45);

        var generated = (string)type.GetMethod("ToDisplayString")!.Invoke(person, null)!;
#pragma warning disable IL2026 // the baseline is reflection on purpose
        var reflected = ReflectionBaseline.ToDisplayString(person);
#pragma warning restore IL2026

        Assert.Equal(generated, reflected);
    }

    [Fact]
    public void ShouldFlagTheReflectionBaselineAsNotTrimSafe()
    {
        var method = typeof(ReflectionBaseline).GetMethod(nameof(ReflectionBaseline.ToDisplayString))!;

        Assert.NotNull(method.GetCustomAttribute<System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute>());
    }

    [Fact]
    public void ShouldReportADiagnosticForANonPartialClass()
    {
        var source = "using Gen; [AutoToString] public class Closed { public int X { get; set; } }";

        var result = Generate(source);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("AUTOTS001", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Contains("Closed", diagnostic.GetMessage());
        Assert.DoesNotContain(result.GeneratedSources, s => s.HintName == "Closed.g.cs");
    }

    [Fact]
    public void ShouldNotReportDiagnosticsForAPartialClass()
    {
        var result = Generate(Person);

        Assert.Empty(result.Diagnostics);
        Assert.Contains(result.GeneratedSources, s => s.HintName == "Person.g.cs");
    }

    private static ImmutableArray<IncrementalGeneratorRunStep> ModelSteps(GeneratorDriver driver) =>
        driver.GetRunResult().Results[0].TrackedSteps[AutoToStringGenerator.ModelStepName];

    [Fact]
    public void ShouldReuseTheCachedModelWhenAnUnrelatedFileChanges()
    {
        var person = Tree(Person, "Person.cs");
        var other = Tree("class Other { }", "Other.cs");
        var driver = NewDriver(tracking: true).RunGenerators(Compile(person, other));

        var edited = Tree("class Other { int changed; }", "Other.cs");
        driver = driver.RunGenerators(Compile(person, edited));

        var outputs = ModelSteps(driver).SelectMany(s => s.Outputs).ToList();
        Assert.NotEmpty(outputs);
        Assert.All(outputs, o => Assert.True(
            o.Reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
            $"Expected a reused model but got {o.Reason}"));
    }

    [Fact]
    public void ShouldRecomputeTheModelWhenTheMarkedClassChanges()
    {
        var person = Tree(Person, "Person.cs");
        var driver = NewDriver(tracking: true).RunGenerators(Compile(person));

        var edited = Tree(Person.Replace("public int Age { get; set; }", "public int Age { get; set; }\n public int Extra { get; set; }"), "Person.cs");
        driver = driver.RunGenerators(Compile(edited));

        var outputs = ModelSteps(driver).SelectMany(s => s.Outputs).ToList();
        Assert.Contains(outputs, o => o.Reason is IncrementalStepRunReason.Modified or IncrementalStepRunReason.New);
        Assert.Contains("Extra", driver.GetRunResult().Results[0].GeneratedSources.Single(s => s.HintName == "Person.g.cs").SourceText.ToString());
    }

    [Fact]
    public void ShouldSkipPropertiesMarkedWithAutoToStringIgnore()
    {
        var source = """
            using Gen;
            [AutoToString]
            public partial class Account
            {
                public string Owner { get; set; } = "";
                [AutoToStringIgnore] public string Password { get; set; } = "";
            }
            """;
        var compilation = Compile(Tree(source));
        NewDriver().RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);

        Assert.DoesNotContain(output.GetDiagnostics(), d => d.Severity == DiagnosticSeverity.Error);
        var text = NewDriver().RunGenerators(compilation).GetRunResult().Results[0].GeneratedSources
            .Single(s => s.HintName == "Account.g.cs").SourceText.ToString();
        Assert.Contains("Owner", text);
        Assert.DoesNotContain("Password", text);
    }
}
