using System.Text.Json;

namespace JsonLab.Tests;

public class JsonCodecTests
{
    [Fact]
    public void ShouldBindPropertiesWhateverTheCasingOfTheNames()
    {
        var order = JsonCodec.Read<Order>("""{"ID":2,"customer":"Bo","total":1,"status":"shipped"}""");

        Assert.Equal(new Order(2, "Bo", 1m, null, Status.Shipped), order);
    }

    [Fact]
    public void ShouldWriteCamelCaseNamesAndStringEnumsAndOmitNulls()
    {
        var json = JsonCodec.Write(new Order(1, "Ana", 9.5m, null, Status.Shipped));

        Assert.Equal("""{"id":1,"customer":"Ana","total":9.5,"status":"shipped"}""", json);
    }

    [Fact]
    public void ShouldKeepANoteThatIsNotNull()
    {
        var json = JsonCodec.Write(new Order(1, "Ana", 9.5m, "gift", Status.Pending));

        Assert.Contains("\"note\":\"gift\"", json);
    }

    [Fact]
    public void ShouldRejectAnEnumWrittenAsANumber()
    {
        Assert.Throws<JsonException>(() => JsonCodec.Read<Order>("""{"id":1,"status":7}"""));
    }

    [Fact]
    public void ShouldRejectAnUnknownMemberWhenReadingStrictly()
    {
        Assert.Throws<JsonException>(() => JsonCodec.ReadStrict<Person>("""{"Name":"a","Age":1,"Extra":1}"""));
    }

    [Fact]
    public void ShouldRejectADuplicatePropertyWhenReadingStrictly()
    {
        Assert.Throws<JsonException>(() => JsonCodec.ReadStrict<Person>("""{"Name":"a","Name":"b","Age":1}"""));
    }

    [Fact]
    public void ShouldRejectAMissingConstructorArgumentWhenReadingStrictly()
    {
        Assert.Throws<JsonException>(() => JsonCodec.ReadStrict<Person>("""{"Name":"x"}"""));
    }

    [Fact]
    public void ShouldReadAValidPayloadStrictly()
    {
        Assert.Equal(new Person("a", 1), JsonCodec.ReadStrict<Person>("""{"Name":"a","Age":1}"""));
    }

    [Fact]
    public void ShouldWriteTheDiscriminatorFirstWhenTheStaticTypeIsTheBase()
    {
        Assert.Equal("""{"kind":"circle","radius":2}""", JsonCodec.Write<Shape>(new Circle(2)));
    }

    [Fact]
    public void ShouldReadTheRightSubtypeFromTheDiscriminator()
    {
        var shape = JsonCodec.Read<Shape>("""{"kind":"square","side":5}""");

        Assert.Equal(new Square(5), shape);
    }

    [Fact]
    public void ShouldRejectAnUnknownDiscriminator()
    {
        Assert.Throws<JsonException>(() => JsonCodec.Read<Shape>("""{"kind":"triangle","side":5}"""));
    }

    [Fact]
    public void ShouldAcceptCommentsAndTrailingCommasInConfigFiles()
    {
        var json = """
            {
              // who
              "NAME": "a",
              "age": 3, /* years */
            }
            """;

        Assert.Equal(new Person("a", 3), JsonCodec.ReadConfig<Person>(json));
    }

    [Fact]
    public void ShouldRejectCommentsWithTheNormalOptions()
    {
        Assert.Throws<JsonException>(() => JsonCodec.Read<Person>("{\"name\":\"a\", // c\n \"age\":3}"));
    }

    [Fact]
    public void ShouldWriteBigNumbersAsStringsForJavaScriptClients()
    {
        var json = JsonCodec.WriteForJavaScript(new Ledger(9007199254740993, 1.5m));

        Assert.Equal("""{"id":"9007199254740993","amount":"1.5"}""", json);
    }

    [Fact]
    public void ShouldReadQuotedNumbersBackForJavaScriptClients()
    {
        var ledger = JsonCodec.ReadFromJavaScript<Ledger>("""{"id":"9007199254740993","amount":"1.5"}""");

        Assert.Equal(new Ledger(9007199254740993, 1.5m), ledger);
    }
}
