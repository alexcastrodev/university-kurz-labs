# Source Generators and AOT-Friendly Code

## Goal

Write a small incremental source generator and see why generators are the AOT-friendly alternative to runtime reflection. For every class marked with `[AutoToString]`, your generator emits a plain C# `ToDisplayString()` method with no reflection in it. The tests drive the generator in memory with `CSharpGeneratorDriver`, read the generated text, compile and run the result, check the diagnostics, and check that unrelated edits reuse the cached pipeline.

## Prerequisites

- C# basics, attributes and `partial` classes
- The idea that reflection and runtime code generation are invisible to the trimmer and to Native AOT
- .NET 10 SDK (the lab uses the `Microsoft.CodeAnalysis.CSharp` 5.0.0 package)

## Task

`AutoToStringGenerator` is an `IIncrementalGenerator`. It lives in a library that targets `net10.0` so the tests can reference it directly. A real analyzer package must target `netstandard2.0` and be referenced with `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`.

The pipeline has four small steps, each one is a method you fill in: the marker attribute is added after initialization, `ForAttributeWithMetadataName` finds the marked classes, a transform turns each one into a small value-equatable `ClassModel`, and an output step renders the source or reports a diagnostic. `ReflectionBaseline` is the runtime-reflection version of the same feature, so you can compare them. Tests assert on substrings, hint names and diagnostic ids, never on whole files, timing or performance.

## Instructions

Complete the following TODOs:

- TODO-00: `CreateModel`: read the `INamedTypeSymbol` and build the `ClassModel` (namespace, name, is it `partial`, public instance properties with a getter). Only simple values go into the model, never symbols, syntax nodes or locations.
- TODO-01: `Render`: return the C# text of `partial class X` with `public string ToDisplayString() => $"X { A={A}, B={B} }";`. Plain string building, no reflection in the generated code.
- TODO-02: `AddAttribute`: add the marker attribute source with `context.AddSource` so user code can use `[AutoToString]` without a reference.
- TODO-03: `BuildPipeline`: use `ForAttributeWithMetadataName` (a cheap syntax predicate, then the transform) and name the step with `WithTrackingName`. This is what makes the incrementality tests possible: an unrelated edit must give `Cached` or `Unchanged`.
- TODO-04: `Emit`: for a marked class that is not `partial`, report the `AUTOTS001` diagnostic and generate nothing. Otherwise add the generated source as `{Name}.g.cs`.
- TODO-05: `ReflectionBaseline.ToDisplayString`: write the same feature with `GetType().GetProperties()`. The method carries `[RequiresUnreferencedCode]`, which is what the IL2026 analyzer reports to callers. The generated code needs no such attribute.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): support `[AutoToStringIgnore]` on properties. Add a second attribute (`Gen.AutoToStringIgnoreAttribute`, targeting properties, hint name `AutoToStringIgnoreAttribute.g.cs`) in `AddAttribute` and skip properties that carry it in `CreateModel`. Then, outside the tests, add `<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>` to a consuming project and read the file your generator produced.
