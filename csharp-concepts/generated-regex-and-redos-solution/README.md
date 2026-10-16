# Generated Regex and ReDoS (Solution)

Official solution for the `generated-regex-and-redos` lab, with the optional tasks implemented.

## What changed from the starter

- `Patterns`: every regex is a `partial` method (or the get-only `partial` property) with `[GeneratedRegex]`. `Legacy` keeps `matchTimeoutMilliseconds: 200`, `Safe` is `^a+$` (same language as `^(a+)+$`, no ambiguity) and `Linear` uses `RegexOptions.NonBacktracking`.
- `InputValidator.Check`: length cap first, then the legacy pattern with `RegexMatchTimeoutException` mapped to `TimedOut`.

## Why it is shaped this way

- Generation does not fix the algorithm: the generated `Legacy` still times out on the hostile input. The timeout bounds the damage, the rewrite removes it.
- `NonBacktracking` compiles without complaint, but the generator cannot emit custom code for it, so `Linear()` is a plain `Regex`. Check the type when you chose the generator for AOT.
- The length cap runs before the regex, so a huge body never reaches the engine.
- A timeout is an error to reject, not something to retry with a slower engine.

## Bonus notes

- `DateProp` and `Date()` both return a cached generated instance. `CountDates` uses `Regex.Count`.
- `.editorconfig` raises `SYSLIB1045` (suggest `[GeneratedRegex]`) to a warning, so a `new Regex("\\d+")` call shows up in the build output.

## Running

```bash
dotnet test
```
