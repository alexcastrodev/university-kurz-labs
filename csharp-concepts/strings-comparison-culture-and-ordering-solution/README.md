# Strings: Comparison, Culture, and Ordering (Solution)

Official solution for the `strings-comparison-culture-and-ordering` lab.

## What changed from the starter

- `SortForPerson` uses `StringComparer.Create(culture, CompareOptions.None)`. The same words
  sort differently per culture: in `en-US` the accented words sit next to their base letters,
  in `sv-SE` the letter `ä` comes after `z`.
- `SortOrdinal` uses `StringComparer.Ordinal`: code unit order, so every uppercase letter comes
  before every lowercase one and accented letters come last.
- `SameIdentifier` uses `StringComparison.OrdinalIgnoreCase`: case-insensitive without a culture
  lookup, the right default for identifiers, file extensions and HTTP headers. It still
  distinguishes `é` from `e` plus a combining accent.
- `SameText` uses `StringComparison.InvariantCulture`: canonically equivalent sequences are
  equal and a soft hyphen has zero weight. Exactly right for human text, wrong for a path or
  a security check.
- `LowerProtocol` uses `ToLowerInvariant`; `ToLower()` would produce a dotless `i` under `tr-TR`.
- `NaturalOrder` compares each run of digits by value (`CompareOptions.NumericOrdering`, new in
  .NET 10). `7` and `07` compare equal, so a stable sort keeps their input order. Signs and
  decimal points are not part of a number, so this is not a version sorter.
- `ContainsIgnoringAccents` calls the invariant `CompareInfo.IndexOf` with
  `IgnoreNonSpace | IgnoreCase`.
- `Canonical` is `string.Intern`. Interned strings live for the whole process, so do not intern
  user input.

## Bonus solutions

- TODO-08 has no code. In invariant globalization mode the cultural methods turn ordinal,
  `new CultureInfo("tr-TR")` throws, and `NumericOrdering` does not throw: it quietly returns the
  plain character difference, so the list is sorted wrongly with no exception.

## Running

```bash
dotnet test
```
