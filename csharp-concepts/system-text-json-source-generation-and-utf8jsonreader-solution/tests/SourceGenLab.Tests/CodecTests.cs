using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SourceGenLab.Tests;

public class CodecTests
{
    private static readonly Order Sample = new(1, "Ana", 9.5m, ["a", "b"], Status.Shipped);

    private static byte[] Utf8(string s) => Encoding.UTF8.GetBytes(s);

    // TODO-00
    [Fact]
    public void Serialize_UsesWebCasingAndStringEnums()
    {
        Assert.Equal("""{"id":1,"customer":"Ana","total":9.5,"tags":["a","b"],"status":"Shipped"}""",
            Codec.Serialize(Sample));
    }

    [Fact]
    public void Deserialize_BindsLowercasePayload()
    {
        var order = Codec.Deserialize("""{"id":7,"customer":"Bia","total":3.25,"tags":["x"],"status":"Pending"}""");

        Assert.NotNull(order);
        Assert.Equal(7, order.Id);
        Assert.Equal("Bia", order.Customer);
        Assert.Equal(3.25m, order.Total);
        Assert.Equal(Status.Pending, order.Status);
    }

    // TODO-01
    [Fact]
    public void SerializeAll_WritesArray()
    {
        var json = Codec.SerializeAll([Sample, Sample with { Id = 2, Tags = [] }]);

        Assert.Equal(
            """[{"id":1,"customer":"Ana","total":9.5,"tags":["a","b"],"status":"Shipped"},""" +
            """{"id":2,"customer":"Ana","total":9.5,"tags":[],"status":"Shipped"}]""", json);
    }

    [Fact]
    public void IsRegistered_ListedAndDiscoveredTypesYes_CollectionsAndForgottenTypesNo()
    {
        Assert.True(Codec.IsRegistered(typeof(Order)));
        Assert.True(Codec.IsRegistered(typeof(Order[])));
        Assert.True(Codec.IsRegistered(typeof(Status))); // discovered through Order's members
        Assert.False(Codec.IsRegistered(typeof(List<Order>))); // collections are not discovered
        Assert.False(Codec.IsRegistered(typeof(Person)));
    }

    [Fact]
    public void ForgottenType_FailsLoudlyThroughTheContext()
    {
        Assert.False(Codec.IsRegistered(typeof(Person)));
        Assert.Throws<InvalidOperationException>(() =>
            JsonSerializer.Serialize(new Person("Ana"), typeof(Person), AppJsonContext.Default));
    }

    // TODO-02
    [Fact]
    public void Fallback_UsesReflectionForUnlistedTypes()
    {
        Assert.Equal("""{"name":"Ana"}""", Codec.SerializeWithFallback(new Person("Ana"), typeof(Person)));
    }

    [Fact]
    public void Fallback_StillUsesGeneratedCodeForListedTypes()
    {
        Assert.Equal("""{"id":1,"customer":"Ana","total":9.5,"tags":["a","b"],"status":"Shipped"}""",
            Codec.SerializeWithFallback(Sample, typeof(Order)));
    }

    // TODO-03
    [Fact]
    public void ReadTokens_ReportsEachTokenInOrder()
    {
        var tokens = Codec.ReadTokens(Utf8("""{"a":[1,true,null],"b":"x"}"""));

        Assert.Equal(
        [
            JsonTokenType.StartObject,
            JsonTokenType.PropertyName, JsonTokenType.StartArray,
            JsonTokenType.Number, JsonTokenType.True, JsonTokenType.Null, JsonTokenType.EndArray,
            JsonTokenType.PropertyName, JsonTokenType.String,
            JsonTokenType.EndObject
        ], tokens);
    }

    [Fact]
    public void ReadTokens_TrailingComma_ThrowsJsonException()
    {
        Assert.ThrowsAny<JsonException>(() => Codec.ReadTokens(Utf8("""{"a":1,}""")));
    }

    [Fact]
    public void SumPaid_SumsOnlyPaidRows()
    {
        var json = Utf8("""[{"status":"paid","total":10.5},{"status":"open","total":99},{"status":"paid","total":0.25}]""");

        Assert.Equal(10.75m, Codec.SumPaid(json));
    }

    [Fact]
    public void SumPaid_IgnoresKeyOrderNestedObjectsAndEscapedNames()
    {
        var json = Utf8(
            """[{"total":5,"meta":{"status":"paid","total":1000},"status":"paid"},""" +
            """{"tags":["paid"],"total":7,"status":"paid"},""" +
            """{"total":3,"status":"unpaid"}]""");

        Assert.Equal(12m, Codec.SumPaid(json));
    }

    [Fact]
    public void SumPaid_KeepsDecimalPrecision()
    {
        var json = Utf8("""[{"status":"paid","total":12345678901234567890.123456789}]""");

        Assert.Equal(12345678901234567890.123456789m, Codec.SumPaid(json));
    }

    // TODO-04
    [Fact]
    public void WriteSummary_WritesCompactJsonIntoTheBuffer()
    {
        var sink = new ArrayBufferWriter<byte>();

        Codec.WriteSummary(sink, [Sample, Sample with { Id = 2, Customer = "Bia", Total = 1m }], relaxedEncoder: false);

        Assert.Equal(
            """{"count":2,"orders":[{"id":1,"customer":"Ana","total":9.5},{"id":2,"customer":"Bia","total":1}]}""",
            Encoding.UTF8.GetString(sink.WrittenSpan));
    }

    [Fact]
    public void WriteSummary_DefaultEncoderEscapes_RelaxedKeepsTextReadable()
    {
        var order = Sample with { Customer = "Ação <b>" };

        var strict = new ArrayBufferWriter<byte>();
        Codec.WriteSummary(strict, [order], relaxedEncoder: false);
        var relaxed = new ArrayBufferWriter<byte>();
        Codec.WriteSummary(relaxed, [order], relaxedEncoder: true);

        var strictText = Encoding.UTF8.GetString(strict.WrittenSpan);
        Assert.Contains("A\\u00E7\\u00E3o \\u003Cb\\u003E", strictText);
        Assert.Contains("\"customer\":\"Ação <b>\"", Encoding.UTF8.GetString(relaxed.WrittenSpan));
    }

    [Fact]
    public void WriteSummary_UsesInvariantNumberFormatting()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            var sink = new ArrayBufferWriter<byte>();
            Codec.WriteSummary(sink, [Sample], relaxedEncoder: false);
            Assert.Contains("\"total\":9.5}", Encoding.UTF8.GetString(sink.WrittenSpan));
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [Fact]
    public void WriteInvalid_ValidatingWriterThrows()
    {
        Assert.Throws<InvalidOperationException>(() => Codec.WriteInvalid(skipValidation: false));
    }

    [Fact]
    public void WriteInvalid_SkipValidationWritesBrokenJson()
    {
        Assert.Equal("{1}", Codec.WriteInvalid(skipValidation: true));
    }

    // TODO-05
    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(16)]
    [InlineData(1000)]
    public void ReadTokensInChunks_MatchesWholeBufferRead(int chunkSize)
    {
        var json = Utf8("""{"id":12345,"name":"Ana Maria","tags":["a","bb"],"ok":true,"none":null,"n":-1.5e3}""");

        Assert.Equal(Codec.ReadTokens(json), Codec.ReadTokensInChunks(json, chunkSize));
    }

    [Fact]
    public void ReadTokensInChunks_InvalidJsonStillThrows()
    {
        Assert.ThrowsAny<JsonException>(() => Codec.ReadTokensInChunks(Utf8("""{"a":1,}"""), 3));
    }
}
