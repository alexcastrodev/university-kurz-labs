# Native AOT and Its Limits

## Goal

See what Native AOT asks of your code without ever running the native compiler. You write the
project files that switch the AOT analyzers on, predict which warning (`IL2026` or `IL3050`) each
risky snippet raises, replace the risky code with AOT-safe code (a System.Text.Json source-generated
context, plain generics), and mark your own dynamic-code method with `[RequiresDynamicCode]` to
watch the warning move to its callers.

## Prerequisites

- The concept text on Native AOT and its limits
- Basic `.csproj` structure and what a NuGet-free class library looks like
- .NET 10 SDK (10.0.100)
- Optional background: the `source-generators-and-aot-friendly-code` lab and the
  `system-text-json-source-generation-and-utf8jsonreader` lab cover source generators and the JSON
  context in depth, so this lab only uses them as the AOT-safe answer

## Task

`AotFiles` returns text: project files and small C# files. The tests write that text into a
throwaway folder and use the real .NET CLI in two ways only:

1. `dotnet msbuild -getProperty:...` to **evaluate** a project (no restore, no build) and read what
   `PublishAot=true` and `IsAotCompatible=true` switch on.
2. `dotnet build` of a plain framework-dependent library with `IsAotCompatible=true`, which runs the
   trim and AOT analyzers and prints `IL2026` / `IL3050` warnings. The tests parse those warnings.

**This lab does not publish a native binary.** `dotnet publish -p:PublishAot=true` downloads the
ILCompiler runtime pack (hundreds of MB) and needs a native linker toolchain (see below), so the
tests must stay offline and fast. Everything here works with the SDK that is already installed, and
the tests refuse to build any project that sets `PublishAot`. The analyzers, which are what
you rely on day to day, give you the same warnings at build time.

## Instructions

This is the official solution: every TODO in `src/NativeAotLab/AotFiles.cs` is implemented. Try the starter first.

The TODOs (`Snippets.cs` is given):

- TODO-00: `PublishAotProject` is a console app with `PublishAot` true. The test checks that the
  three analyzers and `PublishTrimmed` follow from it, and that `InvariantGlobalization` does not.
- TODO-01: `AotCompatibleLibrary` uses `IsAotCompatible` instead: analyzers on, no `PublishAot`.
- TODO-02: `WarningIdsForMakeGenericType`, `WarningIdsForLoadFrom` and `WarningIdsForJsonReflection`
  return the warning IDs you expect for the three given snippets. The test builds them and compares.
- TODO-03: `JsonSourceGeneratedSnippet` replaces the reflection-based `JsonSerializer.Serialize`
  with a `JsonSerializerContext` and the overloads that take a `JsonTypeInfo`. No warnings, and the
  compiled library must round-trip a `Todo`.
- TODO-04: `GenericFactorySnippet` removes runtime `MakeGenericType` / `Array.CreateInstance` in
  favour of generic methods whose type argument the compiler sees.
- TODO-05: `DynamicCodeWrapperSnippet` keeps `MakeGenericType` inside a method annotated with
  `[RequiresDynamicCode]`. The warning disappears from the body and appears on the caller that does
  not repeat the annotation.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

The tests start the .NET CLI roughly twenty times (project evaluations plus seven analyzer builds),
so expect roughly 15 to 30 seconds (about 12 s on the reference machine). Nothing is downloaded and no network is used.

To look at the analyzers on your own code:

```bash
dotnet build -p:IsAotCompatible=true
dotnet msbuild MyApp.csproj -p:PublishAot=true -getProperty:EnableAotAnalyzer
```

### The real publish (manual follow-up, not part of the tests)

```bash
dotnet publish -c Release -r osx-arm64 -p:PublishAot=true
```

Replace the runtime identifier with yours (`linux-x64`, `win-x64`, ...). It needs network access for
the ILCompiler package on first use and a native toolchain: Xcode Command Line Tools on macOS,
`clang` and zlib headers on Linux, the "Desktop development with C++" workload on Windows. Native AOT
does not cross-compile between operating systems. Remember that the analyzers cannot see everything
(for example a missing `JsonSerializerContext` registration publishes clean), so run the published
binary.

## Bonus (Optional)

- TODO-06 (optional): `StrictAotLibrary` makes `IL2026` and `IL3050` build errors with
  `WarningsAsErrors`. The test builds a dynamic-code snippet (must fail with `error IL3050`) and a
  clean one (must pass).
- TODO-07 (optional): `GuardedFactorySnippet` calls a `[RequiresDynamicCode]` method only inside
  `if (RuntimeFeature.IsDynamicCodeSupported)`, which the analyzer understands as a guard.
