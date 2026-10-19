# NuGet Restore, Versions and Dependency Resolution

## Goal

Build a tiny dependency resolver over an in-memory package feed and see the restore rules as code: SemVer ordering, range notation, the lowest applicable version, floating versions, cousin dependencies, "nearest wins", and the diagnostics you know as `NU1605` (downgrade) and `NU1107` (conflict). No network, no real restore.

## Prerequisites

- C# basics, LINQ
- The idea of `PackageReference`, transitive dependencies and `obj/project.assets.json`
- .NET 10 SDK (the lab uses the `NuGet.Versioning` 6.12.1 package: `NuGetVersion`, `VersionRange`, `VersionComparer`)

## Task

`Feed` is an in-memory feed (package id to versions to dependencies) and `Dep` is an edge with a range written as in a project file (`"1.0"`, `"[1.0]"`, `"1.*"`). It is already complete. `VersionRules` and `Resolver` hold the rules. `Pick` and `Run` are already written and call your functions. Tests build small feeds in code and assert on resolved versions and on the messages your diagnostics produce. Nothing depends on time, the network or the file system.

## Instructions

Complete the following TODOs:

- TODO-00: `VersionRules.Compare` and `VersionRules.Satisfies`: SemVer ordering (`1.0.0-beta.2 < 1.0.0`, `1.0 == 1.0.0`, build metadata ignored) and range notation (`1.0` means `>= 1.0`, `[1.0]` exact, `(1.0,2.0)` exclusive, `[1.0,2.0)`).
- TODO-01: `PickLowest`: the "lowest applicable version" rule. A range does not mean "the newest", it means the lowest available version that satisfies it.
- TODO-02: `PickFloating`: a floating range (`1.*`) takes the highest match and ignores pre-release versions.
- TODO-03: `PickForGroup`: cousin dependencies, the same package requested by several branches at the same depth. The lowest version satisfying every request wins, or null.
- TODO-04: `Resolve`: walk the graph breadth first. The nearest request decides each package (a direct dependency beats a transitive one), and deeper requests for an already resolved id are ignored.
- TODO-05: `Diagnose`: check every edge against the resolved versions and report `NU1101`, `NU1102`, `NU1605` (the direct reference won with a lower version) and `NU1107` (conflict).

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): `Why`: the shortest chain from a direct reference to a transitive package, like `dotnet nuget why`.
- TODO-07 (optional): `CheckLocked`: compare a lock file's contents with a fresh resolution and return `NU1004` when they differ, the idea behind `--locked-mode`.
