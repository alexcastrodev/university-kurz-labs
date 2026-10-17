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

    public static string LibraryProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Greeter</PackageId>
            <AssemblyName>Greeter</AssemblyName>
            <VersionPrefix>1.2.3</VersionPrefix>
            <VersionSuffix>beta.4</VersionSuffix>
            <Authors>Contoso</Authors>
            <Description>Greets people.</Description>
            <PackageLicenseExpression>MIT</PackageLicenseExpression>
            <PackageProjectUrl>https://example.com/greeter</PackageProjectUrl>
            <PackageTags>greeting;sample</PackageTags>
            <RepositoryUrl>https://example.com/greeter.git</RepositoryUrl>
            <RepositoryType>git</RepositoryType>
          </PropertyGroup>
        </Project>
        """;

    public static string LibraryProjectWithReadmeAndSymbols() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Greeter</PackageId>
            <Version>1.0.0</Version>
            <Authors>Contoso</Authors>
            <Description>Greets people.</Description>
            <PackageReadmeFile>README.md</PackageReadmeFile>
            <IncludeSymbols>true</IncludeSymbols>
            <SymbolPackageFormat>snupkg</SymbolPackageFormat>
          </PropertyGroup>
          <ItemGroup>
            <None Include="README.md" Pack="true" PackagePath="\" />
          </ItemGroup>
        </Project>
        """;

    public static string NotPackableProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <IsPackable>false</IsPackable>
          </PropertyGroup>
        </Project>
        """;

    public static string MultiTargetProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFrameworks>net10.0;netstandard2.1</TargetFrameworks>
            <PackageId>Contoso.Greeter</PackageId>
            <Version>1.0.0</Version>
          </PropertyGroup>
        </Project>
        """;

    public static string BuildTransitiveProps() => """
        <Project>
          <PropertyGroup>
            <GreeterEnabled>true</GreeterEnabled>
          </PropertyGroup>
        </Project>
        """;

    public static string ProjectWithBuildTransitive() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Greeter</PackageId>
            <Version>1.0.0</Version>
          </PropertyGroup>
          <ItemGroup>
            <None Include="Contoso.Greeter.props" Pack="true" PackagePath="buildTransitive\Contoso.Greeter.props" />
          </ItemGroup>
        </Project>
        """;

    public static string DependentProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Extras</PackageId>
            <Version>1.0.0</Version>
          </PropertyGroup>
          <ItemGroup>
            <ProjectReference Include="..\Contoso.Core\Contoso.Core.csproj" />
          </ItemGroup>
        </Project>
        """;

    public static string ReproducibleProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <Deterministic>true</Deterministic>
            <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
            <PublishRepositoryUrl>true</PublishRepositoryUrl>
            <EmbedUntrackedSources>true</EmbedUntrackedSources>
          </PropertyGroup>
        </Project>
        """;

    public static string PackOnBuildProject() => """
        <Project Sdk="Microsoft.NET.Sdk">
          <PropertyGroup>
            <TargetFramework>net10.0</TargetFramework>
            <PackageId>Contoso.Greeter</PackageId>
            <Version>1.0.0</Version>
            <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
          </PropertyGroup>
        </Project>
        """;
}
