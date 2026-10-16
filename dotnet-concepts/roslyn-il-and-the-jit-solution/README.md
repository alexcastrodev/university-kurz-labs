# Roslyn, IL and the JIT

## Goal

Use the C# compiler as a library: compile source text in memory, run the result, then read the IL that Roslyn produced and the metadata that describes it. See with your own eyes that `checked` becomes `add.ovf`, that Debug and Release produce different IL, and that the compiler (syntax and semantic model) and the JIT hints are all inspectable data.

## Prerequisites

- C# basics and reflection
- The idea that C# compiles to IL and the JIT compiles IL to machine code on first call
- .NET 10 SDK (the lab uses the `Microsoft.CodeAnalysis.CSharp` 5.0.0 package)

## Task

`CompilerPipeline` compiles C# text to an assembly image (`byte[]`) and runs a method from it. `IlInspector` reads the IL bytes of a method straight from the metadata and turns them into a list of opcodes. `SyntaxAndJit` queries syntax trees, the semantic model, and the `MethodImplAttributes` that carry JIT hints. Tests assert on opcodes and diagnostics only, never on byte sizes or timing, so they do not depend on the compiler version.

## Instructions

Every TODO is implemented in this solution. The list below explains each one:

- TODO-00: `Compile`: `CSharpSyntaxTree.ParseText`, `CSharpCompilation.Create` with the trusted platform assemblies as references, `Emit` to a `MemoryStream`. Errors come back as diagnostics (for example `CS0029`), not as exceptions.
- TODO-01: `Run`: load the image into a collectible `AssemblyLoadContext`, invoke a static method by reflection, and always unload the context.
- TODO-02: `ReadOpCodes`: walk the IL bytes with `System.Reflection.Emit.OpCodes`, handling the two-byte prefix `0xFE` and the operand size of each opcode.
- TODO-03: `GetMethodIl`: use `PEReader` and `MetadataReader` to find a method definition and return its IL body without loading the assembly.
- TODO-04: `DeclaredMethodNames` (syntax tree only) and `TypeOfInitializer` (semantic model): see the difference between what the text says and what the compiler knows.
- TODO-05: `GetImplFlags`: read `NoInlining` and `AggressiveInlining` from `GetMethodImplementationFlags()`. They are metadata hints for the JIT, readable without running anything.

All tests pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `GetEntryPointName`: read the entry point token from the PE header and find the method name. For top-level statements it is the compiler-generated `<Main>$`.
- TODO-07 (optional): `ForceJit`: call `RuntimeHelpers.PrepareMethod` so the method is compiled before its first call. Then, outside the tests, run your app with `DOTNET_TieredCompilation=0` and with `DOTNET_TieredPGO=0` and write down which parts of the JIT pipeline from the concept you changed. Do not measure time in a unit test: it is not deterministic.
