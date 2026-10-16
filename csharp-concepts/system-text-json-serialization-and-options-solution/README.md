# System.Text.Json: Serialization and Options (Solution)

Official solution for the `system-text-json-serialization-and-options` lab.

## What changed from the starter

- `Options` starts from `JsonSerializerDefaults.Web`, sets
  `DefaultIgnoreCondition = WhenWritingNull` and registers one
  `JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)`. It is a
  `static readonly` field: the options are built once and become read-only on first use.
  A new converter or naming policy object per call would defeat the metadata cache.
- `StrictOptions` is `JsonSerializerOptions.Strict`: unmapped members and duplicate properties
  are errors, matching is case-sensitive, and nullable annotations and required constructor
  parameters are respected.
- `Shape` gets `[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]` and one
  `[JsonDerivedType]` per subtype with an explicit string, so the wire format does not depend
  on type names. The discriminator must come first in the object when reading.
- `ConfigOptions` and `JavaScriptOptions` use the copy constructor `new(Options) { ... }`. A
  copy is a new instance, but it shares the same converter objects, so it does not rebuild
  the converter.
- `JsonNumberHandling.WriteAsString | AllowReadingFromString` quotes `long` and `decimal` on
  write and accepts quoted numbers on read.

## Bonus solutions

- TODO-05 has no code. The shared instance is the baseline; a fresh instance with a new
  converter object each call rebuilds the type metadata every time, which is much slower and
  allocates far more.

## Running

```bash
dotnet test
```
