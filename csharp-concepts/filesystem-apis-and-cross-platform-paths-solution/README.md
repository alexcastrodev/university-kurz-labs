# Filesystem APIs and Cross-Platform Paths (Solution)

Official solution for the `filesystem-apis-and-cross-platform-paths` lab.

## What changed from the starter

- `Path.Combine("data", "/etc/passwd")` returns `/etc/passwd` because a rooted piece restarts the path. `Path.Join` only concatenates. Neither stops `..`, so `SafeJoin` resolves with `GetFullPath` and checks the prefix against the base path plus a trailing separator (without it, `<base>2` would pass as "inside" `<base>`).
- `EnumerationOptions` replaces `SearchOption`: `MatchCasing.CaseInsensitive` gives the same answer on every platform, and `MaxRecursionDepth = 0` limits the search to the top folder.
- By default `AttributesToSkip` is only `Hidden | System`, so directory links are followed and the same file is found twice. Adding `ReparsePoint` skips them.
- `SaveAtomically` writes a sibling temp file (same volume, so the move is a rename), flushes to disk, then `File.Move(tmp, path, overwrite: true)`. The `catch` removes the temp file and rethrows, so the old file survives a crash in the middle.
- `FileShare.None` makes the second open fail with `IOException`; `FileNotFoundException` is also an `IOException`, so it is rethrown explicitly to avoid hiding a missing file.
- `RandomAccess` takes an explicit offset and has no stream position, so parallel writes to different indexes need no lock. Reading past the end returns 0 bytes, which is why `Read` validates the index first.

## Running

```bash
dotnet test
```
