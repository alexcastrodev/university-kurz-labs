using System.Text.Json;

namespace JsonLab;

public static class JsonCodec
{
    // TODO-00: Build the shared options ONCE, starting from JsonSerializerDefaults.Web
    //   (camelCase names, case-insensitive reads, quoted numbers accepted). On top of that:
    //   omit null properties when writing, and write enums as camelCase strings while
    //   rejecting enum numbers when reading (JsonStringEnumConverter with allowIntegerValues: false).
    //   Create the converter once, never per call.
    public static readonly JsonSerializerOptions Options = new();

    // TODO-01: Use the .NET 10 strict preset: unmapped members, duplicate properties and
    //   missing constructor arguments must all fail.
    public static readonly JsonSerializerOptions StrictOptions = new();

    // TODO-03: Config files may have comments and trailing commas. Derive these options from
    //   Options with the copy constructor, then skip comments and allow trailing commas.
    public static readonly JsonSerializerOptions ConfigOptions = new();

    // TODO-04: JavaScript clients lose precision above 2^53. Derive these options from Options
    //   and write numbers as strings, while still accepting quoted numbers on read.
    public static readonly JsonSerializerOptions JavaScriptOptions = new();

    public static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);

    public static T? Read<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);

    public static T? ReadStrict<T>(string json) => JsonSerializer.Deserialize<T>(json, StrictOptions);

    public static T? ReadConfig<T>(string json) => JsonSerializer.Deserialize<T>(json, ConfigOptions);

    public static string WriteForJavaScript<T>(T value) => JsonSerializer.Serialize(value, JavaScriptOptions);

    public static T? ReadFromJavaScript<T>(string json) => JsonSerializer.Deserialize<T>(json, JavaScriptOptions);
}
