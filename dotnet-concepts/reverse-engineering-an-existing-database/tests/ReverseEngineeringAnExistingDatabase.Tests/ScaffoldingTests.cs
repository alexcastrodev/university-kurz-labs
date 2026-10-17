using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ReverseEngineeringAnExistingDatabase.Tests;

/// <summary>Each test builds a throw-away legacy database file in its own temp directory.</summary>
public sealed class ScaffoldingTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "legacy-" + Guid.NewGuid().ToString("N"));
    private readonly string _connectionString;

    public ScaffoldingTests()
    {
        Directory.CreateDirectory(_dir);
        _connectionString = LegacySchema.Create(Path.Combine(_dir, "legacy.db"));
    }

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    private IReadOnlyDictionary<string, string> Scaffold(ScaffoldSettings? settings = null)
        => Scaffolding.Scaffold(_connectionString, settings ?? new ScaffoldSettings());

    // Compiles the scaffolded files plus extra hand-written files, loads them and returns the EF model.
    private static IModel BuildModel(IReadOnlyDictionary<string, string> scaffolded, params string[] handWritten)
    {
        var trees = scaffolded.Values.Concat(handWritten).Select(code => CSharpSyntaxTree.ParseText(code));
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            "Scaffolded" + Guid.NewGuid().ToString("N"), trees, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        Assert.True(result.Success, string.Join(Environment.NewLine,
            result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));

        stream.Position = 0;
        var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
        var contextType = assembly.GetType("Legacy.Models.LegacyDb")!;
        var builderType = typeof(DbContextOptionsBuilder<>).MakeGenericType(contextType);
        var builder = (DbContextOptionsBuilder)Activator.CreateInstance(builderType)!;
        builder.UseSqlite("Data Source=:memory:");
        var options = builder.Options;
        var context = (DbContext)Activator.CreateInstance(contextType, options)!;
        return context.Model;
    }

    // TODO-00
    [Fact]
    public void Scaffold_ShouldProduceOneClassPerTableAndOneContext()
    {
        var files = Scaffold();

        Assert.Equal(
            ["AuditLog.cs", "Customer.cs", "LegacyDb.cs", "OrderHdr.cs", "OrderLine.cs", "Tag.cs", "VOrderTotal.cs"],
            files.Keys.Order());
        Assert.Contains("public partial class LegacyDb : DbContext", files["LegacyDb.cs"]);
    }

    [Fact]
    public void Scaffold_ShouldCleanNamesAndKeepTheOriginalOnesInTheMapping()
    {
        var files = Scaffold();

        Assert.Contains("public int CustId { get; set; }", files["Customer.cs"]);
        Assert.Contains("DbSet<Customer> Customers", files["LegacyDb.cs"]);
        Assert.Contains("entity.ToTable(\"CUSTOMER\")", files["LegacyDb.cs"]);
        Assert.Contains(".HasColumnName(\"cust_id\")", files["LegacyDb.cs"]);
    }

    [Fact]
    public void Scaffold_ShouldNotWriteTheConnectionStringIntoTheSource()
    {
        var files = Scaffold();

        Assert.DoesNotContain("OnConfiguring", files["LegacyDb.cs"]);
        Assert.DoesNotContain("legacy.db", files["LegacyDb.cs"]);
    }

    [Fact]
    public void Scaffold_ShouldTurnTheJoinTableIntoSkipNavigationsInsteadOfAClass()
    {
        var files = Scaffold();

        Assert.False(files.ContainsKey("OrderTag.cs"));
        Assert.Contains("UsingEntity<Dictionary<string, object>>", files["LegacyDb.cs"]);
        Assert.Contains("ICollection<Tag> Tags", files["OrderHdr.cs"]);
    }

    [Fact]
    public void Scaffold_ShouldReadCompositeKeysDefaultsAndNullability()
    {
        var files = Scaffold();

        Assert.Contains("HasKey(e => new { e.OrdId, e.LineNo })", files["LegacyDb.cs"]);
        Assert.Contains(".HasDefaultValue(\"A\")", files["LegacyDb.cs"]);
        Assert.Contains(".HasDefaultValueSql(\"CURRENT_TIMESTAMP\")", files["LegacyDb.cs"]);
        Assert.Contains("IsRequired()", files["LegacyDb.cs"]);
        Assert.Contains("OnDelete(DeleteBehavior.ClientSetNull)", files["LegacyDb.cs"]);
    }

    // TODO-01
    [Fact]
    public void DatabaseNames_ShouldKeepTheNamesExactlyAsInTheDatabase()
    {
        var files = Scaffold(Presets.DatabaseNames());

        Assert.True(files.ContainsKey("CUSTOMER.cs"));
        Assert.False(files.ContainsKey("Customer.cs"));
        Assert.Contains("public int cust_id { get; set; }", files["CUSTOMER.cs"]);
        Assert.Contains("DbSet<CUSTOMER> CUSTOMER", files["LegacyDb.cs"]);
    }

    // TODO-02
    [Fact]
    public void DataAnnotations_ShouldMoveWhatAttributesCanExpressIntoTheClass()
    {
        var files = Scaffold(Presets.DataAnnotations());

        Assert.Contains("[Table(\"CUSTOMER\")]", files["Customer.cs"]);
        Assert.Contains("[Column(\"cust_id\")]", files["Customer.cs"]);
        Assert.Contains("[Key]", files["Customer.cs"]);
        Assert.Contains("[Keyless]", files["AuditLog.cs"]);
        Assert.DoesNotContain("HasColumnName(\"cust_id\")", files["LegacyDb.cs"]);
    }

    [Fact]
    public void DataAnnotations_ShouldStillNeedTheFluentApiForWhatAttributesCannotSay()
    {
        var files = Scaffold(Presets.DataAnnotations());

        Assert.Contains("OnDelete(DeleteBehavior.ClientSetNull)", files["LegacyDb.cs"]);
        Assert.Contains("ValueGeneratedNever()", files["LegacyDb.cs"]);
        Assert.Contains("UsingEntity<Dictionary<string, object>>", files["LegacyDb.cs"]);
    }

    // TODO-03
    [Fact]
    public void OnlyTables_ShouldScaffoldJustTheRequestedOnes()
    {
        var files = Scaffold(Presets.OnlyTables("CUSTOMER", "ORDER_HDR"));

        Assert.Equal(["Customer.cs", "LegacyDb.cs", "OrderHdr.cs"], files.Keys.Order());
        Assert.DoesNotContain("OrderLine", files["LegacyDb.cs"]);
    }

    [Fact]
    public void OnlyTables_ShouldAcceptAViewByName()
    {
        var files = Scaffold(Presets.OnlyTables("V_ORDER_TOTALS"));

        Assert.Equal(["LegacyDb.cs", "VOrderTotal.cs"], files.Keys.Order());
        Assert.Contains(".ToView(\"V_ORDER_TOTALS\")", files["LegacyDb.cs"]);
    }

    // TODO-04
    [Fact]
    public void Findings_ShouldListTheTableWithoutPrimaryKeyAndTheViewAsKeyless()
    {
        Assert.Equal(["AuditLog", "VOrderTotal"], Findings.KeylessEntities(Scaffold()));
    }

    [Fact]
    public void Findings_ShouldFindTheViewColumnThatBecameByteArray()
    {
        Assert.Equal(["VOrderTotal.TotalSpent"], Findings.ByteArrayProperties(Scaffold()));
    }

    [Fact]
    public void Findings_ShouldFindNothingKeylessWhenOnlyKeyedTablesAreScaffolded()
    {
        var files = Scaffold(Presets.OnlyTables("CUSTOMER", "TAG"));

        Assert.Empty(Findings.KeylessEntities(files));
        Assert.Empty(Findings.ByteArrayProperties(files));
    }

    // TODO-05
    [Fact]
    public void PartialExtension_ShouldCompileTogetherWithTheScaffoldedCode()
    {
        BuildModel(Scaffold(), PartialExtension.Source("Legacy.Models"));
    }

    [Fact]
    public void PartialExtension_ShouldOverrideTheGeneratedDeleteBehavior()
    {
        var plain = BuildModel(Scaffold());
        var extended = BuildModel(Scaffold(), PartialExtension.Source("Legacy.Models"));

        Assert.Equal(DeleteBehavior.ClientSetNull, DeleteOfOrderLine(plain));
        Assert.Equal(DeleteBehavior.Cascade, DeleteOfOrderLine(extended));
    }

    [Fact]
    public void PartialExtension_ShouldSurviveARescaffold()
    {
        var first = Scaffold();
        using (var conn = new Microsoft.Data.Sqlite.SqliteConnection(_connectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE CUSTOMER ADD COLUMN phone TEXT";
            cmd.ExecuteNonQuery();
        }
        var second = Scaffold();

        Assert.DoesNotContain("Phone", first["Customer.cs"]);
        Assert.Contains("string Phone", second["Customer.cs"]);
        BuildModel(second, PartialExtension.Source("Legacy.Models"));
    }

    private static DeleteBehavior? DeleteOfOrderLine(IModel model)
        => model.FindEntityType("Legacy.Models.OrderLine")!.GetForeignKeys().Single().DeleteBehavior;

    // TODO-06
    [Fact]
    public void ScaffoldToDirectory_ShouldWriteFilesAndRefuseToOverwriteWithoutForce()
    {
        var output = Path.Combine(_dir, "Models");

        Scaffolding.ScaffoldToDirectory(_connectionString, new ScaffoldSettings(), output, force: false);

        Assert.True(File.Exists(Path.Combine(output, "LegacyDb.cs")));
        Assert.ThrowsAny<Exception>(() =>
            Scaffolding.ScaffoldToDirectory(_connectionString, new ScaffoldSettings(), output, force: false));
    }

    [Fact]
    public void ScaffoldToDirectory_ShouldOverwriteGeneratedFilesWithForce()
    {
        var output = Path.Combine(_dir, "Models");
        Scaffolding.ScaffoldToDirectory(_connectionString, new ScaffoldSettings(), output, force: false);
        File.AppendAllText(Path.Combine(output, "Customer.cs"), "// my edit");

        Scaffolding.ScaffoldToDirectory(_connectionString, new ScaffoldSettings(), output, force: true);

        Assert.DoesNotContain("// my edit", File.ReadAllText(Path.Combine(output, "Customer.cs")));
    }

    // TODO-07
    [Fact]
    public void SourceWithGeneratedKeys_ShouldFixTheGuessedValueGeneratedNever()
    {
        var plain = BuildModel(Scaffold(), PartialExtension.Source("Legacy.Models"));
        var fixedKeys = BuildModel(Scaffold(), PartialExtension.SourceWithGeneratedKeys("Legacy.Models"));

        Assert.Equal(ValueGenerated.Never, CustomerKey(plain).ValueGenerated);
        Assert.Equal(ValueGenerated.OnAdd, CustomerKey(fixedKeys).ValueGenerated);
        Assert.Equal(DeleteBehavior.Cascade, DeleteOfOrderLine(fixedKeys));
    }

    private static IProperty CustomerKey(IModel model)
        => model.FindEntityType("Legacy.Models.Customer")!.FindProperty("CustId")!;
}
