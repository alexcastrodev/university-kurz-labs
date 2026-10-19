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
        throw new NotImplementedException("TODO-00");

    // TODO-01: Return a console project that publishes as ONE self-contained executable for `rid`:
    //          RuntimeIdentifier = rid, SelfContained true, PublishSingleFile true and
    //          IncludeNativeLibrariesForSelfExtract true. Do not set the analyzer property:
    //          PublishSingleFile already turns EnableSingleFileAnalyzer on.
    public static string SelfContainedSingleFileProject(string rid) =>
        throw new NotImplementedException("TODO-01");

    // TODO-02: Return a self-contained, trimmed console project for `rid`: PublishTrimmed true,
    //          TrimMode set to `trimMode` ("partial" or "full") and one <TrimmerRootAssembly Include="..." />
    //          per entry of `rootAssemblies` (assemblies ILLink must never trim).
    public static string TrimmedProject(string rid, string trimMode, params string[] rootAssemblies) =>
        throw new NotImplementedException("TODO-02");

    // TODO-03: Return a trimmed, self-contained project for `rid` that also cuts what the app does not
    //          need: InvariantGlobalization true, EventSourceSupport false, UseSystemResourceKeys true.
    //          Because trimming turns HTTP/3 off in .NET 10, opt back in with Http3Support true, and
    //          show every warning inside packages with TrimmerSingleWarn false.
    public static string SlimRuntimeProject(string rid) =>
        throw new NotImplementedException("TODO-03");

    // TODO-06 (optional): Return a self-contained single-file project for `rid` that is precompiled and
    //          compressed: PublishReadyToRun true and EnableCompressionInSingleFile true.
    public static string ReadyToRunCompressedProject(string rid) =>
        throw new NotImplementedException("TODO-06");
}
