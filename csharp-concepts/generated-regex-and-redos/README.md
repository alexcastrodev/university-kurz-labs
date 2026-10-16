# Generated Regex and ReDoS

## Goal

Declare regular expressions with `[GeneratedRegex]`, see when the generator falls back to a plain `Regex`, and defend a validator against catastrophic backtracking (ReDoS) with a match timeout, a safer pattern and an input length cap.

## Prerequisites

- Basic `System.Text.RegularExpressions`
- C# `partial` methods
- .NET 10 SDK

## Task

`Patterns` holds four regexes that are stubbed or still built the old way. `InputValidator` checks untrusted text against the vulnerable legacy pattern `^(a+)+$`. A 41 character hostile input must not pin a CPU core: the tests never rely on slow runs, only on the timeout firing or on the fixed pattern answering at once.

## Instructions

Complete the following TODOs:

- TODO-00: `Date()`: a generated regex with named groups `y`, `m`, `d`.
- TODO-01: `Legacy()`: generate the vulnerable pattern and keep its 200 ms timeout.
- TODO-02: `Safe()`: the same accepted strings, no catastrophic backtracking.
- TODO-03: `Linear()`: the same text on the non-backtracking engine. Which type do you get back?
- TODO-04: `InputValidator.Check`: reject input over `MaxLength` first.
- TODO-05: `InputValidator.Check`: turn a regex timeout into `ValidationResult.TimedOut`.

Run the tests until they all pass.

## Running the Lab

From the lab directory:

```bash
dotnet test
```

## Bonus (Optional)

- TODO-06 (optional): Add a get-only `partial Regex DateProp { get; }` with the same pattern as `Date()` (partial properties, C# 13).
- TODO-07 (optional): Add `Patterns.CountDates(string text)` using `Regex.Count` on `Date()`.
- TODO-08 (optional): Add an `.editorconfig` that raises `SYSLIB1045` to a warning, then write a `new Regex("\\d+")` call and read the build output.
