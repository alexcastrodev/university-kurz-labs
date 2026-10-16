using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonLab;

public static class JsonCodec
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) },
    };

    public static readonly JsonSerializerOptions StrictOptions = JsonSerializerOptions.Strict;

    public static readonly JsonSerializerOptions ConfigOptions = new(Options)
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static readonly JsonSerializerOptions JavaScriptOptions = new(Options)
    {
        NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString,
    };

    public static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? Read<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);

    public static T? ReadStrict<T>(string json) => JsonSerializer.Deserialize<T>(json, StrictOptions);

    public static T? ReadConfig<T>(string json) => JsonSerializer.Deserialize<T>(json, ConfigOptions);

    public static string WriteForJavaScript<T>(T value) => JsonSerializer.Serialize(value, JavaScriptOptions);

    public static T? ReadFromJavaScript<T>(string json) => JsonSerializer.Deserialize<T>(json, JavaScriptOptions);
}
