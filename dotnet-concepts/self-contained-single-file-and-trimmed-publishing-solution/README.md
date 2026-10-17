# Self-Contained, Single-File and Trimmed Publishing

Official solution for the `self-contained-single-file-and-trimmed-publishing` lab.

## Goal

Describe the publish switches in MSBuild (framework-dependent, self-contained with a RuntimeIdentifier,
single file, trimming, feature switches, ReadyToRun) and see the trim and single-file analyzers
catch the code that would break in a published app, then fix that code.

## Prerequisites

- The concept text on self-contained, single-file and trimmed publishing
- Basic `.csproj` structure (`PropertyGroup`, `ItemGroup`)
- .NET 10 SDK (10.0.100)

## Task

`ProjectFiles` and `AnalyzerFiles` return the text of `.csproj` files and of a small C# program.
The tests write that text into a throwaway folder and use the .NET CLI in two ways:

- `dotnet msbuild App.csproj -getProperty:... -getItem:...` evaluates the project (no restore, no build)
  and tells which values MSBuild computed, including the ones the SDK infers for you
  (`PublishSingleFile` turns on `EnableSingleFileAnalyzer`, `PublishTrimmed` turns on `EnableTrimAnalyzer`).
- `dotnet build` of a framework-dependent project (no RuntimeIdentifier) runs the Roslyn trim and
  single-file analyzers, so the IL warning ids show up in the build output.

**This lab does not run `dotnet publish`.** A real self-contained, single-file or trimmed publish needs a
runtime pack (about 70 MB), and trimming or ReadyToRun also need the ILLink and Crossgen2 packs, all
downloaded from NuGet. The tests must run offline and quickly, so a RuntimeIdentifier only appears in
evaluation and never reaches restore. The analyzers give you the same warnings at build time without
any pack.

## Instructions

Every TODO below is implemented in this solution:

- TODO-00: `ProjectFiles.FrameworkDependentProject` (no RID, `SelfContained` false, `UseAppHost` false).
- TODO-01: `ProjectFiles.SelfContainedSingleFileProject` (RID, `SelfContained`, `PublishSingleFile`,
  `IncludeNativeLibrariesForSelfExtract`; the analyzer is inferred).
- TODO-02: `ProjectFiles.TrimmedProject` (`PublishTrimmed`, `TrimMode`, `TrimmerRootAssembly` items).
- TODO-03: `ProjectFiles.SlimRuntimeProject` (feature switches, `Http3Support`, `TrimmerSingleWarn`).
- TODO-04: `AnalyzerFiles.AnalyzerProject` (analyzers on; building `SampleCode.RiskyProgram` reports
  IL2026, IL2057, IL2070 and IL3000).
- TODO-05: `AnalyzerFiles.FixedProgram` (`typeof(Plugin)`, `[DynamicallyAccessedMembers]`, a source-generated
  `JsonSerializerContext`, `AppContext.BaseDirectory`).

All tests pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

It runs about 8 CLI invocations and takes roughly 4 to 10 seconds. No network is used.

Manual follow-up (needs network the first time, because it downloads packs). In any console project:

```bash
dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true
dotnet publish -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishReadyToRun=true
```

Compare the sizes of the output folders, and run the published binary of `SampleCode.RiskyProgram`
to see `type=` and `json=` break when trimmed.

## Bonus (Optional)

- TODO-06: `ProjectFiles.ReadyToRunCompressedProject` (`PublishReadyToRun`, `EnableCompressionInSingleFile`).
- TODO-07: `AnalyzerFiles.StrictAnalyzerProject` (`WarningsAsErrors` for IL2026, IL2057, IL2070 makes the build fail).
