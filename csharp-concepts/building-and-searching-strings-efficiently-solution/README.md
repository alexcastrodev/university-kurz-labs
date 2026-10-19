# Building and Searching Strings Efficiently

## Goal

Build text without a pile of garbage (`StringBuilder`, `string.Create`) and search it with vectorized, ordinal `SearchValues`.

## Prerequisites

- C# strings, `Span<char>` and `ReadOnlySpan<char>`
- Lambdas and `static` lambdas
- .NET 10 SDK

## Task

`Text` is stubbed. You will build a CSV, pre-size a builder and stream its chunks, format a fixed-width tag straight into its final string, and scan text for forbidden characters and keywords.

## Instructions

Complete the following TODOs:

- TODO-00: `BuildCsv`: a `StringBuilder` instead of `+=` in a loop.
- TODO-01: `CreateBuilder` and `WriteChunks`: pre-size the builder, and write its chunks without `ToString()`.
- TODO-02: `FormatTag`: `string.Create` with the data as state, so only the result string is allocated.
- TODO-03: `IndexOfForbidden`: a `static readonly SearchValues<char>` created once.
- TODO-04: `IndexOfKeyword`: a `SearchValues<string>` with `OrdinalIgnoreCase`, unaffected by the current culture.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Rent a `char[]` from `ArrayPool<char>.Shared` to build a string of a known size, slice to the length you need, and return it in a `finally`. Why can you not rely on `buffer.Length` or on zeroed contents?
- TODO-06 (optional): Replace `FormatTag` with `Span<char>.TryWrite` over a small `stackalloc` buffer. What happens when the text does not fit?
