using System.Buffers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SourceGenLab;

public static class Codec
{
    public static string Serialize(Order order) =>
        JsonSerializer.Serialize(order, AppJsonContext.Default.Order);

    public static Order? Deserialize(string json) =>
        JsonSerializer.Deserialize(json, AppJsonContext.Default.Order);

    public static string SerializeAll(IEnumerable<Order> orders) =>
        JsonSerializer.Serialize(orders.ToArray(), AppJsonContext.Default.OrderArray);

    public static bool IsRegistered(Type type) => AppJsonContext.Default.GetTypeInfo(type) is not null;

    // Built once: options become read-only on first use and cache the metadata.
    private static readonly JsonSerializerOptions FallbackOptions = new(JsonSerializerDefaults.Web)
    {
        TypeInfoResolver = JsonTypeInfoResolver.Combine(AppJsonContext.Default, new DefaultJsonTypeInfoResolver())
    };

    public static string SerializeWithFallback(object value, Type type) =>
        JsonSerializer.Serialize(value, type, FallbackOptions);

    public static List<JsonTokenType> ReadTokens(ReadOnlySpan<byte> utf8Json)
    {
        var tokens = new List<JsonTokenType>();
        var reader = new Utf8JsonReader(utf8Json);
        while (reader.Read())
            tokens.Add(reader.TokenType);
        return tokens;
    }

    public static decimal SumPaid(ReadOnlySpan<byte> utf8Json)
    {
        var reader = new Utf8JsonReader(utf8Json);
        decimal sum = 0m;
        bool paid = false;
        decimal total = 0m;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.PropertyName && reader.CurrentDepth == 2)
            {
                if (reader.ValueTextEquals("status"u8))
                {
                    reader.Read();
                    paid = reader.TokenType == JsonTokenType.String && reader.ValueTextEquals("paid"u8);
                }
                else if (reader.ValueTextEquals("total"u8))
                {
                    reader.Read();
                    total = reader.GetDecimal();
                }
                else
                {
                    reader.Read();
                    reader.Skip(); // no-op on scalars, jumps over nested objects and arrays
                }
            }
            else if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == 1)
            {
                if (paid) sum += total;
                paid = false;
                total = 0m;
            }
        }
        return sum;
    }

    public static void WriteSummary(IBufferWriter<byte> sink, IReadOnlyList<Order> orders, bool relaxedEncoder)
    {
        var options = new JsonWriterOptions
        {
            Encoder = relaxedEncoder ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
        };
        using var writer = new Utf8JsonWriter(sink, options);
        writer.WriteStartObject();
        writer.WriteNumber("count", orders.Count);
        writer.WriteStartArray("orders");
        foreach (var o in orders)
        {
            writer.WriteStartObject();
            writer.WriteNumber("id", o.Id);
            writer.WriteString("customer", o.Customer);
            writer.WriteNumber("total", o.Total);
            writer.WriteEndObject();
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();
    }

    public static string WriteInvalid(bool skipValidation)
    {
        var sink = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(sink, new JsonWriterOptions { SkipValidation = skipValidation });
        writer.WriteStartObject();
        writer.WriteNumberValue(1);
        writer.WriteEndObject();
        writer.Flush();
        return Encoding.UTF8.GetString(sink.WrittenSpan);
    }

    public static List<JsonTokenType> ReadTokensInChunks(byte[] utf8Json, int chunkSize)
    {
        var tokens = new List<JsonTokenType>();
        var state = default(JsonReaderState);
        int offset = 0, available = 0;
        while (true)
        {
            available = Math.Min(utf8Json.Length, available + chunkSize);
            bool final = available == utf8Json.Length;
            var reader = new Utf8JsonReader(utf8Json.AsSpan(offset, available - offset), final, state);
            while (reader.Read())
                tokens.Add(reader.TokenType);
            offset += (int)reader.BytesConsumed;
            state = reader.CurrentState;
            if (final) break;
        }
        return tokens;
    }
}
