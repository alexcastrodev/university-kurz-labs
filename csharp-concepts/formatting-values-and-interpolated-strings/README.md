# Formatting Values: Interpolated Strings, Format Specifiers and Cultures

## Goal

Format numbers and your own types with alignment and format strings, keep machine-readable text independent of the machine culture, and write an interpolated string handler that skips work when logging is off.

## Prerequisites

- C# string interpolation basics
- `CultureInfo` and `IFormatProvider`
- .NET 10 SDK

## Task

`Formatter`, `Money` and `DebugHandler` are stubbed. Every test pins its culture (`en-US`, `pt-BR`, `de-DE`), so the result does not depend on your machine settings. Run the tests before you start: a pt-BR machine prints `0,5` where an en-US one prints `0.5`, and that is exactly the bug this lab is about.

## Instructions

Complete the following TODOs:

- TODO-00: `Row`: hole syntax `{value,width:format}` with a culture you receive.
- TODO-01: `CacheKey`: data must use the invariant culture.
- TODO-02: `Compact`: custom format with `0` and `#` placeholders, and the zero trap.
- TODO-03: `Money.ToString(format, provider)`: your own `IFormattable` format strings.
- TODO-04: `DebugHandler` constructor: `out bool shouldAppend`, so disabled logs never evaluate their holes.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-05 (optional): Make `Money` implement `ISpanFormattable` and write it into a `Span<char>` with `TryWrite`.
- TODO-06 (optional): Add `Formatter.Greeting(name, count, culture)` using a `static readonly CompositeFormat` parsed once.
