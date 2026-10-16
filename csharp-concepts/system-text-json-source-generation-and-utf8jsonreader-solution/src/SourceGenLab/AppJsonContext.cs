using System.Text.Json;
using System.Text.Json.Serialization;

namespace SourceGenLab;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Order[]))]
public partial class AppJsonContext : JsonSerializerContext;
