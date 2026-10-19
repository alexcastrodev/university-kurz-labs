# Filesystem APIs and Cross-Platform Paths

## Goal

Handle the edges of `System.IO`: build paths from untrusted input without escaping a folder, enumerate trees with `EnumerationOptions` (case, depth, symbolic links), replace files without ever leaving them half-written, use `FileShare` to refuse concurrent opens, and read and write records by offset with `RandomAccess`.

## Prerequisites

- C# classes, `IEnumerable<T>` and LINQ basics
- `FileStream` basics (`using`, `FileMode`)
- .NET 10 SDK

## Task

`FileOps` and `RecordFile` are stubbed. Every test creates its own unique temp folder and deletes it afterwards, so nothing is written outside the temp folder and tests do not depend on the current directory.

Portability notes: assertions never hard-code `/` or `\`, they build expected values with `Path.Combine`. File name case sensitivity depends on the file system, so TODO-01 asks for an explicit `MatchCasing`. Creating symbolic links on Windows may need privileges or developer mode; the link test returns early when the link cannot be created. On Unix, `FileShare` is emulated with advisory locks (it only protects against other .NET code), on Windows it is enforced by the OS; the test stays inside one process, where both behave the same.

## Instructions

Complete the following TODOs:

- TODO-00: `SafeJoin`: `Path.Join` instead of `Path.Combine`, then resolve and confine the result.
- TODO-01: `FindFiles`: case-insensitive recursive search with a maximum depth, returning relative paths.
- TODO-02: `CountByName`: skip symbolic links with `AttributesToSkip` or follow them.
- TODO-03: `SaveAtomically`: temp file in the same folder, then `File.Move(..., overwrite: true)`; clean up on failure.
- TODO-04: `TryOpenExclusive`: `FileShare.None` and a `null` result when the file is already open.
- TODO-05: `RecordFile`: offset-based reads and writes with `File.OpenHandle` and `RandomAccess`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Write a helper that returns a `FileStream` on a new temp file that deletes itself when disposed (`FileOptions.DeleteOnClose`, `Path.GetRandomFileName()`, `FileMode.CreateNew`). Why is that better than `Path.GetTempFileName()`?
- TODO-07 (optional): Write one test that proves `Directory.EnumerateFiles` is lazy and `Directory.GetFiles` is a snapshot (create a file between the call and the iteration). When would you choose each one?
