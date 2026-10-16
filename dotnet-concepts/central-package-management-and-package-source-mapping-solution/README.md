# Central Package Management and Package Source Mapping

Official solution for the `central-package-management-and-package-source-mapping` lab.

## Goal

Write the files that turn dependency handling into a repository policy: a
`Directory.Packages.props` that centralizes versions, project files that reference packages
without a version, and a `nuget.config` with package source mapping. Then apply the documented
source mapping rule yourself.

## Prerequisites

- `PackageReference` and basic MSBuild items and properties
- NuGet sources and `nuget.config`
- .NET 10 SDK

## Task

The methods in `PackageFiles` and `SourceMapping` return the text of the files. The tests write
that text into a temporary directory and check it in two ways:

- MSBuild files are evaluated through the .NET CLI with
  `dotnet msbuild <project> -getItem:... -getProperty:...`. This evaluates the project without
  restore and without network, so you see exactly which items and properties MSBuild computes.
- Package source mapping is resolved by NuGet at restore time and cannot be evaluated offline.
  The tests parse the XML and use your `SourceMapping.Resolve`, which applies the documented
  matching rule: an exact id beats a prefix, a longer prefix beats a shorter one, `*` is the
  weakest, and a package matching no pattern gets no source.

## Instructions

Every TODO below is implemented in this solution:

- TODO-00: Implement `PackageFiles.DirectoryPackagesProps` (enable CPM, declare `PackageVersion` items).
- TODO-01: Implement `PackageFiles.ProjectFile` (`PackageReference` without `Version`, `VersionOverride` where requested).
- TODO-02: Implement `PackageFiles.NestedDirectoryPackagesProps` (import the parent file with `GetPathOfFileAbove`).
- TODO-03: Implement `PackageFiles.GlobalPackagesProps` (`GlobalPackageReference` items).
- TODO-04: Implement `SourceMapping.NuGetConfig` (sources plus `packageSourceMapping`).
- TODO-05: Implement `SourceMapping.Resolve` (most specific pattern wins, no match means no source).

All tests pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

The tests call `dotnet msbuild` for evaluation only, so they need the .NET 10 SDK but no network.

## Bonus (Optional)

- TODO-06 (optional): Implement `PackageFiles.TransitivePinningProps` with
  `CentralPackageTransitivePinningEnabled`.
- TODO-07 (optional): Implement `SourceMapping.Unmapped` and think about why an unmapped package
  makes restore fail with `NU1100` once any mapping exists.
