# Formatting Values: Interpolated Strings, Format Specifiers and Cultures (Solution)

Official solution for the `formatting-values-and-interpolated-strings` lab.

## What changed from the starter

- `string.Create(culture, $"{name,-10}{price,10:N2}")` passes the culture to the handler, so the same code gives `1,234.50` or `1.234,50` on demand. A negative width left-aligns, a positive one right-aligns.
- `CacheKey` uses `CultureInfo.InvariantCulture`: `ToString()` would follow the machine and write `1,5` on a pt-BR one. Parsing has the mirror problem, so always pass the culture on both sides.
- `ToString("0.##")` replaces `#.##`: `#` never invents a digit, so zero would become an empty string.
- `Money.ToString(format, provider)` decides what each format string means and rejects unknown ones with `FormatException`, because format strings are only checked at run time.
- `shouldAppend = logger.Enabled` makes the compiler skip every `Append*` call and the hole expressions themselves. The cost is a trap: keep holes free of side effects.
- Bonus: `ISpanFormattable.TryFormat` lets `Span<char>.TryWrite` fill a caller-provided buffer without a result string (this sample still builds a string inside, a real one formats the pieces straight into the span), and `CompositeFormat.Parse` parses a reused template once.

## Running

```bash
dotnet test
```
