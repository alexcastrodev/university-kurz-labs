namespace PublishingLab;

/// <summary>
/// Each method returns the text of a .csproj. The tests write it to a temporary folder and ask
/// `dotnet msbuild -getProperty` what MSBuild evaluated. Nothing is restored or published.
/// </summary>
public static class ProjectFiles
{
    // TODO-00: Return an SDK-style console project (OutputType Exe, net10.0) that is framework-dependent:
    //          no RuntimeIdentifier, SelfContained false, and UseAppHost false (no native launcher).
    public static string FrameworkDependentProject() =>
        """
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <SelfContained>false</SelfContained>
            <UseAppHost>false</UseAppHost>
          </PropertyGroup>

        </Project>
        """;

    // TODO-01: Return a console project that publishes as ONE self-contained executable for `rid`:
    //          RuntimeIdentifier = rid, SelfContained true, PublishSingleFile true and
    //          IncludeNativeLibrariesForSelfExtract true. Do not set the analyzer property:
    //          PublishSingleFile already turns EnableSingleFileAnalyzer on.
    public static string SelfContainedSingleFileProject(string rid) =>
        $"""
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <RuntimeIdentifier>{rid}</RuntimeIdentifier>
            <SelfContained>true</SelfContained>
            <PublishSingleFile>true</PublishSingleFile>
            <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
          </PropertyGroup>

        </Project>
        """;

    // TODO-02: Return a self-contained, trimmed console project for `rid`: PublishTrimmed true,
    //          TrimMode set to `trimMode` ("partial" or "full") and one <TrimmerRootAssembly Include="..." />
    //          per entry of `rootAssemblies` (assemblies ILLink must never trim).
    public static string TrimmedProject(string rid, string trimMode, params string[] rootAssemblies)
    {
        var roots = string.Join(Environment.NewLine,
            rootAssemblies.Select(a => $"""    <TrimmerRootAssembly Include="{a}" />"""));
        return $"""
            <Project Sdk="Microsoft.NET.Sdk">

              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net10.0</TargetFramework>
                <RuntimeIdentifier>{rid}</RuntimeIdentifier>
                <SelfContained>true</SelfContained>
                <PublishTrimmed>true</PublishTrimmed>
                <TrimMode>{trimMode}</TrimMode>
              </PropertyGroup>

              <ItemGroup>
            {roots}
              </ItemGroup>

            </Project>
            """;
    }

    // TODO-03: Return a trimmed, self-contained project for `rid` that also cuts what the app does not
    //          need: InvariantGlobalization true, EventSourceSupport false, UseSystemResourceKeys true.
    //          Because trimming turns HTTP/3 off in .NET 10, opt back in with Http3Support true, and
    //          show every warning inside packages with TrimmerSingleWarn false.
    public static string SlimRuntimeProject(string rid) =>
        $"""
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <RuntimeIdentifier>{rid}</RuntimeIdentifier>
            <SelfContained>true</SelfContained>
            <PublishTrimmed>true</PublishTrimmed>
            <InvariantGlobalization>true</InvariantGlobalization>
            <EventSourceSupport>false</EventSourceSupport>
            <UseSystemResourceKeys>true</UseSystemResourceKeys>
            <Http3Support>true</Http3Support>
            <TrimmerSingleWarn>false</TrimmerSingleWarn>
          </PropertyGroup>

        </Project>
        """;

    // TODO-06 (optional): Return a self-contained single-file project for `rid` that is precompiled and
    //          compressed: PublishReadyToRun true and EnableCompressionInSingleFile true.
    public static string ReadyToRunCompressedProject(string rid) =>
        $"""
        <Project Sdk="Microsoft.NET.Sdk">

          <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net10.0</TargetFramework>
            <RuntimeIdentifier>{rid}</RuntimeIdentifier>
            <SelfContained>true</SelfContained>
            <PublishSingleFile>true</PublishSingleFile>
            <PublishReadyToRun>true</PublishReadyToRun>
            <EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
          </PropertyGroup>

        </Project>
        """;
}
