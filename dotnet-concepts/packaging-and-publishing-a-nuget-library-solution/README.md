# Packaging and Publishing a NuGet Library

## Goal

Make a library into a NuGet package by editing only the project file: package metadata,
a version built from `VersionPrefix` and `VersionSuffix`, a readme and a symbols package,
a project that opts out of packing, multi-targeting, a `buildTransitive` props file and a
`ProjectReference` that becomes a dependency. Then open the produced `.nupkg` and see for
yourself what is inside.

## Prerequisites

- Basic `.csproj` structure (`PropertyGroup`, `ItemGroup`)
- The concept text on packaging and publishing a NuGet library
- .NET 10 SDK

## Task

`PackagingFiles` returns the text of project files (and one props file). Each test writes your
text into a throwaway folder, adds a tiny `.cs` file, runs `dotnet pack -c Release -o out`,
opens the resulting `.nupkg` with `System.IO.Compression.ZipFile` and asserts on the `.nuspec`
and on the list of files in the zip. A package is just a zip with a manifest, so you can do the
same by hand with `unzip -l`.

Everything is offline: the projects have no `PackageReference`, nothing is pushed to any feed
and nothing is downloaded. Targets are `net10.0` and `netstandard2.1`, both covered by the
reference packs that ship with the SDK.

## Instructions

Complete the following TODOs in `src/PackagingLab/PackagingFiles.cs`:

- TODO-00: `LibraryProject` sets the package metadata and builds the version `1.2.3-beta.4` from
  `VersionPrefix` and `VersionSuffix`. `PackageId` differs from `AssemblyName`.
- TODO-01: `LibraryProjectWithReadmeAndSymbols` declares the readme twice (property plus a
  `None` item with `Pack="true"`) and asks for an `.snupkg` symbols package.
- TODO-02: `NotPackableProject` opts out of packing with `IsPackable`.
- TODO-03: `MultiTargetProject` targets two frameworks and gets two `lib/` folders.
- TODO-04: `BuildTransitiveProps` and `ProjectWithBuildTransitive` ship a props file under
  `buildTransitive/` so consumers inherit its settings.
- TODO-05: `DependentProject` references another project, which turns into a dependency in the
  manifest instead of a bundled DLL.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

Each test runs one `dotnet pack` (or `dotnet build`), so the whole run takes about 20 to 40
seconds. Be patient on the first run.

To look at a package of your own:

```bash
dotnet pack -c Release -o out
unzip -l out/*.nupkg
unzip -p out/*.nupkg '*.nuspec'
```

Publishing is a manual follow-up and is NOT part of this lab (never run it against a real feed
from the tests). With an API key scoped to push only, restricted to your package glob and with
an expiration date:

```bash
dotnet nuget push out/Contoso.Greeter.1.2.3-beta.4.nupkg --source https://api.nuget.org/v3/index.json --api-key <key>
```

NuGet.org also supports trusted publishing (a short-lived key issued to your CI workflow), which
removes the long-lived secret altogether. Read the NuGet documentation before using either.

## Bonus (Optional)

- TODO-06 (optional): `ReproducibleProject` enables `Deterministic`, `ContinuousIntegrationBuild`,
  `PublishRepositoryUrl` and `EmbedUntrackedSources`.
- TODO-07 (optional): `PackOnBuildProject` uses `GeneratePackageOnBuild` so that `dotnet build`
  creates the package too.
