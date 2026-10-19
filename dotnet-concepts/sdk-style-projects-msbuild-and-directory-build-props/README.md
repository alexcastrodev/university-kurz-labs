# SDK-Style Projects, MSBuild and Directory.Build.props

## Goal

Write the MSBuild files that make a modern project work: a six-line SDK-style `.csproj`, an
exclusion from the implicit `Compile` glob, shared settings in `Directory.Build.props`, a
`Directory.Build.targets` that reads what the project chose, a nested props file that chains to
its parent, and a custom target. Then see MSBuild confirm what you wrote.

## Prerequisites

- Basic `.csproj` structure (`PropertyGroup`, `ItemGroup`)
- The concept text on SDK-style projects and `Directory.Build.props`
- .NET 10 SDK

## Task

`MsBuildFiles` returns the text of MSBuild files. Each test writes your text into a throwaway
folder tree, adds a few `.cs` files, and runs `dotnet msbuild -getProperty:... -getItem:Compile`
to read what MSBuild evaluated. Nothing is restored or compiled, and no network is used. The
tests only fail when MSBuild really evaluates something different from what the TODO asks.

## Instructions

Complete the following TODOs in `src/SdkStyleLab/MsBuildFiles.cs`:

- TODO-00: `ClassLibraryProject` returns a minimal SDK-style project with `net10.0`.
- TODO-01: `ProjectWithExcludedFile` removes the `Legacy` folder from the implicit `Compile` glob.
- TODO-02: `DirectoryBuildProps` sets shared properties, and `ProjectOverridingNullable` shows the
  project body winning over it.
- TODO-03: `DirectoryBuildTargets` derives `ProjectKind` from the project's `OutputType`.
- TODO-04: `NestedDirectoryBuildProps` imports the parent file with `GetPathOfFileAbove`.
- TODO-05: `CustomTargetProject` defines a target `WriteStamp` that writes a file.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

To look at what MSBuild sees in any project of your own:

```bash
dotnet msbuild MyProject.csproj -getProperty:Nullable -getItem:Compile
```

## Bonus (Optional)

- TODO-06 (optional): `ReleaseStrictProject` turns on `TreatWarningsAsErrors` only when
  `Configuration` is `Release`.
- TODO-07 (optional): `ExplicitCompileProject` disables the default compile items and lists
  `Program.cs` by hand.
