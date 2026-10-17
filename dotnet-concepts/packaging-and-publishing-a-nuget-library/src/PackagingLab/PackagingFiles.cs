namespace PackagingLab;

/// <summary>
/// The text of the files that turn a library into a NuGet package. The tests write the text to
/// disk, run `dotnet pack` (offline: no project has a PackageReference), open the produced
/// .nupkg as a zip and inspect the manifest and the file list.
/// </summary>
public static class PackagingFiles
{
    public const string ReadmeMarkdown = "# Contoso.Greeter\n\nGreets people.\n";

    // Given (not a TODO): a second packable library used by TODO-05.
    public static string CoreProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Core</PackageId>
            <Version>2.0.0</Version>
          </PropertyGroup>
        </Project>
        """;

    // TODO-00: Return an SDK-style class library (net10.0) with these properties:
    //   PackageId Contoso.Greeter, AssemblyName Greeter (different on purpose),
    //   VersionPrefix 1.2.3 and VersionSuffix beta.4 (NOT Version), Authors Contoso,
    //   Description "Greets people.", PackageLicenseExpression MIT,
    //   PackageProjectUrl https://example.com/greeter, PackageTags "greeting;sample",
    //   RepositoryUrl https://example.com/greeter.git and RepositoryType git.
    public static string LibraryProject() => throw new NotImplementedException();

    // TODO-01: Return a net10.0 library with PackageId Contoso.Greeter, Version 1.0.0, Authors
    //   Contoso and a Description, that (a) names README.md with PackageReadmeFile AND packs the
    //   file with <None Include="README.md" Pack="true" PackagePath="\" />, and (b) creates a
    //   symbols package: IncludeSymbols true and SymbolPackageFormat snupkg.
    public static string LibraryProjectWithReadmeAndSymbols() => throw new NotImplementedException();

    // TODO-02: Return a net10.0 project that must never produce a package (IsPackable false).
    public static string NotPackableProject() => throw new NotImplementedException();

    // TODO-03: Return a library that targets net10.0 and netstandard2.1 with the plural property
    //   TargetFrameworks (separated by a semicolon). PackageId Contoso.Greeter, Version 1.0.0.
    public static string MultiTargetProject() => throw new NotImplementedException();

    // TODO-04: Return the props file that ships inside the package under buildTransitive/. It is
    //   a <Project> (no Sdk) that sets the property GreeterEnabled to true, so every project that
    //   consumes the package, directly or transitively, sees it.
    public static string BuildTransitiveProps() => throw new NotImplementedException();

    // TODO-04 (continued): Return a net10.0 library (PackageId Contoso.Greeter, Version 1.0.0)
    //   that packs the file Contoso.Greeter.props with <None ... Pack="true" PackagePath=...>
    //   so it lands at buildTransitive\Contoso.Greeter.props inside the package.
    public static string ProjectWithBuildTransitive() => throw new NotImplementedException();

    // TODO-05: Return a net10.0 library (PackageId Contoso.Extras, Version 1.0.0) with a
    //   ProjectReference to ..\Contoso.Core\Contoso.Core.csproj. Packing it must list Contoso.Core
    //   as a dependency in the nuspec and must NOT copy Contoso.Core.dll into lib/.
    public static string DependentProject() => throw new NotImplementedException();

    // TODO-06 (optional): Return a net10.0 project that turns on Deterministic,
    //   ContinuousIntegrationBuild, PublishRepositoryUrl and EmbedUntrackedSources (all true),
    //   the properties that make a CI build reproducible and traceable. No extra package needed.
    public static string ReproducibleProject() => throw new NotImplementedException();

    // TODO-07 (optional): Return a net10.0 library (PackageId Contoso.Greeter, Version 1.0.0)
    //   with GeneratePackageOnBuild true, so a plain `dotnet build` also writes the .nupkg to
    //   bin/Release.
    public static string PackOnBuildProject() => throw new NotImplementedException();
}
