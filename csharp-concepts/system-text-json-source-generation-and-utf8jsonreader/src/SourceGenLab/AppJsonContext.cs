using System.Text.Json;
using System.Text.Json.Serialization;

namespace SourceGenLab;

// TODO-00: Turn this into the real source-generated context.
//   1. Add [JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]
//      so casing and enum names are fixed at compile time.
//   2. Register the root types with [JsonSerializable]: Order and Order[]. Then delete the
//      placeholder below (it only exists so the generator has something to emit and the starter compiles).
//   The generator ships with the SDK, no package is needed. Member types (int, string, Status...)
//   are discovered by walking Order, but collections are not: they must be listed.
[JsonSerializable(typeof(Status))]
public partial class AppJsonContext : JsonSerializerContext;
