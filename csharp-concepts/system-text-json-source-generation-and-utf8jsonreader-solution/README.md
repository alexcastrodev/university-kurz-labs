# System.Text.Json: Source Generation and Utf8JsonReader (Solution)

Official solution for the `system-text-json-source-generation-and-utf8jsonreader` lab.

## What changed from the starter

- `AppJsonContext` carries `[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]` and registers `Order` and `Order[]`. Casing and enum names are fixed at compile time. `Default.Order` and `Default.OrderArray` are the generated `JsonTypeInfo` properties, and calls that take them never touch reflection.
- `Status` is found by walking `Order`'s members, but `List<Order>` is not: `GetTypeInfo` returns `null`, which is why `SerializeAll` converts to an array. `Person` was never listed, so serializing it through the context throws `InvalidOperationException`.
- `SerializeWithFallback` builds one `static readonly` options instance with `JsonTypeInfoResolver.Combine(AppJsonContext.Default, new DefaultJsonTypeInfoResolver())`: generated code first, reflection only for unlisted types. That reintroduces trimming and AOT warnings, which is the price of the fallback.
- `ReadTokens` is a plain `while (reader.Read())` loop. A trailing comma makes the reader throw `JsonReaderException` (a `JsonException`).
- `SumPaid` is order independent: it tracks `paid` and `total` per row and commits them at the `EndObject` of depth 1. Property names are compared with `ValueTextEquals("status"u8)`, which does not allocate and understands escapes such as `status`. `Skip()` jumps over nested values so a nested `"status":"paid"` is not mistaken for the row's own. `GetDecimal` keeps the digits that `GetDouble` would lose.
- `WriteSummary` writes to the `IBufferWriter<byte>` and calls `Flush()`. The default encoder escapes non-ASCII and `<`, `>`; `UnsafeRelaxedJsonEscaping` keeps the text readable and is only safe when the output is never embedded in HTML.
- `WriteInvalid` shows the writer validating structure: a number directly inside an object throws `InvalidOperationException`; with `SkipValidation = true` it writes `{1}`.
- `ReadTokensInChunks` re-feeds the unconsumed bytes each round: a new reader over `[offset, available)`, `isFinalBlock` only on the last round, and the previous `CurrentState`. Since unconsumed bytes stay in the window, a token larger than one chunk is simply completed in a later round.

## Bonus solutions

- TODO-06: with `GenerationMode = Serialization` writing `Order` works, but reading throws `InvalidOperationException` ("did not provide property metadata"). Fast-path mode generates only a write handler, and it does not support polymorphism, so keep the default metadata mode unless you only write.
- TODO-07: a `JsonElement` points into the pooled buffer owned by the `JsonDocument`. After `Dispose` any use throws `ObjectDisposedException`; `Clone()` copies the data so it survives.

## Running

```bash
dotnet test
```
