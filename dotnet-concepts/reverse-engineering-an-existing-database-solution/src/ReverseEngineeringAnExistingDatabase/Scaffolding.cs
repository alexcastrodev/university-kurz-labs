using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Sqlite.Design.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace ReverseEngineeringAnExistingDatabase;

/// <summary>What you would pass to <c>dotnet ef dbcontext scaffold</c> as flags.</summary>
public sealed record ScaffoldSettings(
    bool UseDatabaseNames = false,
    bool NoPluralize = false,
    bool UseDataAnnotations = false,
    IReadOnlyList<string>? Tables = null,
    string ContextName = "LegacyDb",
    string Namespace = "Legacy.Models");

public static class Scaffolding
{
    /// <summary>The scaffolder the CLI would build (already done): design-time services plus the SQLite provider.</summary>
    public static IReverseEngineerScaffolder CreateScaffolder()
    {
        var services = new ServiceCollection()
            .AddEntityFrameworkDesignTimeServices();
#pragma warning disable EF1001 // the CLI itself uses this provider class; it is the in-process way to reach it
        new SqliteDesignTimeServices().ConfigureDesignTimeServices(services);
#pragma warning restore EF1001
        return services.BuildServiceProvider().GetRequiredService<IReverseEngineerScaffolder>();
    }

    /// <summary>TODO-00: translate the settings into the scaffolder call (the in-process <c>dotnet ef dbcontext scaffold</c>).</summary>
    public static ScaffoldedModel ScaffoldModel(string connectionString, ScaffoldSettings settings)
        => CreateScaffolder().ScaffoldModel(
            connectionString,
            new DatabaseModelFactoryOptions(settings.Tables ?? [], []),
            new ModelReverseEngineerOptions
            {
                UseDatabaseNames = settings.UseDatabaseNames,
                NoPluralize = settings.NoPluralize
            },
            new ModelCodeGenerationOptions
            {
                ContextName = settings.ContextName,
                ContextNamespace = settings.Namespace,
                ModelNamespace = settings.Namespace,
                UseDataAnnotations = settings.UseDataAnnotations,
                SuppressConnectionStringWarning = true,
                SuppressOnConfiguring = true
            });

    /// <summary>Scaffolds in memory and returns file name to source text, context file included (already done).</summary>
    public static IReadOnlyDictionary<string, string> Scaffold(string connectionString, ScaffoldSettings settings)
    {
        var model = ScaffoldModel(connectionString, settings);
        var files = new Dictionary<string, string> { [model.ContextFile.Path] = model.ContextFile.Code };
        foreach (var file in model.AdditionalFiles)
            files[file.Path] = file.Code;
        return files;
    }

    /// <summary>
    /// TODO-06 (optional): like <c>dotnet ef dbcontext scaffold</c> writing files to disk.
    /// Without <paramref name="force"/> it must refuse to touch existing files (the scaffolder throws).
    /// </summary>
    public static void ScaffoldToDirectory(string connectionString, ScaffoldSettings settings, string directory, bool force)
    {
        var model = ScaffoldModel(connectionString, settings);
        CreateScaffolder().Save(model, directory, overwriteFiles: force);
    }
}
