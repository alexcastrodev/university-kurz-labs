using System.Xml.Linq;

namespace CentralPackageManagementLab.Tests;

public class SourceMappingTests
{
    private const string Config = """
        <configuration>
          <packageSources>
            <clear />
            <add key="internal" value="https://feed.example/internal" />
            <add key="public" value="https://feed.example/public" />
          </packageSources>
          <packageSourceMapping>
            <packageSource key="internal">
              <package pattern="Contoso.*" />
              <package pattern="Fabrikam.Special" />
            </packageSource>
            <packageSource key="contoso-public">
              <package pattern="Contoso.Open.*" />
            </packageSource>
            <packageSource key="public">
              <package pattern="*" />
            </packageSource>
          </packageSourceMapping>
        </configuration>
        """;

    [Fact]
    public void NuGetConfig_declares_sources_and_mappings()
    {
        var xml = SourceMapping.NuGetConfig(
            new Dictionary<string, string> { ["internal"] = "https://feed.example/internal", ["public"] = "https://api.nuget.org/v3/index.json" },
            new Dictionary<string, string[]> { ["internal"] = ["Contoso.*", "Fabrikam.Special"], ["public"] = ["*"] });

        var doc = XDocument.Parse(xml);
        var sources = doc.Root!.Element("packageSources")!;
        Assert.NotNull(sources.Element("clear"));
        Assert.Equal("https://feed.example/internal",
            sources.Elements("add").Single(a => (string?)a.Attribute("key") == "internal").Attribute("value")!.Value);
        Assert.Equal(2, sources.Elements("add").Count());

        var mapping = doc.Root.Element("packageSourceMapping")!;
        var internalPatterns = mapping.Elements("packageSource").Single(s => (string?)s.Attribute("key") == "internal")
            .Elements("package").Select(p => p.Attribute("pattern")!.Value).ToArray();
        Assert.Equal(["Contoso.*", "Fabrikam.Special"], internalPatterns);
        Assert.Equal("*", mapping.Elements("packageSource").Single(s => (string?)s.Attribute("key") == "public")
            .Element("package")!.Attribute("pattern")!.Value);
    }

    [Fact]
    public void Generated_config_resolves_like_the_documented_rule()
    {
        var xml = SourceMapping.NuGetConfig(
            new Dictionary<string, string> { ["internal"] = "a", ["public"] = "b" },
            new Dictionary<string, string[]> { ["internal"] = ["Contoso.*"], ["public"] = ["*"] });

        Assert.Equal("internal", SourceMapping.Resolve(xml, "Contoso.Internal"));
        Assert.Equal("public", SourceMapping.Resolve(xml, "Newtonsoft.Json"));
    }

    [Theory]
    [InlineData("Contoso.Internal", "internal")]
    [InlineData("contoso.internal", "internal")]
    [InlineData("Contoso.Open.Tools", "contoso-public")]
    [InlineData("Fabrikam.Special", "internal")]
    [InlineData("Fabrikam.Other", "public")]
    [InlineData("Newtonsoft.Json", "public")]
    public void Resolve_picks_the_most_specific_pattern(string packageId, string expectedSource)
        => Assert.Equal(expectedSource, SourceMapping.Resolve(Config, packageId));

    [Fact]
    public void Exact_id_beats_a_longer_looking_prefix()
    {
        const string xml = """
            <configuration><packageSourceMapping>
              <packageSource key="a"><package pattern="Contoso.Core" /></packageSource>
              <packageSource key="b"><package pattern="Contoso.Cor*" /></packageSource>
            </packageSourceMapping></configuration>
            """;
        Assert.Equal("a", SourceMapping.Resolve(xml, "Contoso.Core"));
        Assert.Equal("b", SourceMapping.Resolve(xml, "Contoso.Coral"));
    }

    [Fact]
    public void Package_matching_no_pattern_gets_no_source()
    {
        const string xml = """
            <configuration><packageSourceMapping>
              <packageSource key="internal"><package pattern="Contoso.*" /></packageSource>
            </packageSourceMapping></configuration>
            """;
        Assert.Null(SourceMapping.Resolve(xml, "Newtonsoft.Json"));
    }

    [Fact]
    public void Unmapped_lists_ids_without_a_source()
    {
        const string xml = """
            <configuration><packageSourceMapping>
              <packageSource key="internal"><package pattern="Contoso.*" /></packageSource>
            </packageSourceMapping></configuration>
            """;
        Assert.Equal(["Newtonsoft.Json", "Serilog"],
            SourceMapping.Unmapped(xml, ["Contoso.Core", "Newtonsoft.Json", "Serilog"]));
    }
}
