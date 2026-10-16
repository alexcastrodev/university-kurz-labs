namespace SdkStyleLab;

/// <summary>
/// Each method returns the full text of an MSBuild file. The tests write the text to disk
/// and ask the .NET CLI what MSBuild evaluated (no restore, no build, no network).
/// </summary>
public static class MsBuildFiles
{
    // TODO-00: Return a minimal SDK-style class library: <Project Sdk="Microsoft.NET.Sdk"> with
    //   a PropertyGroup holding TargetFramework net10.0. Nothing else: the SDK adds the rest.
    public static string ClassLibraryProject() => throw new NotImplementedException();

    // TODO-01: Same project as TODO-00, plus an ItemGroup that removes everything under the
    //   "Legacy" folder from the implicit Compile glob (Compile Remove="Legacy/**").
    public static string ProjectWithExcludedFile() => throw new NotImplementedException();

    // TODO-02: Return a Directory.Build.props (root element <Project> without Sdk) that sets, for
    //   every project below it: TargetFramework net10.0, Nullable enable, LangVersion 14.0 and
    //   TreatWarningsAsErrors true.
    public static string DirectoryBuildProps() => throw new NotImplementedException();

    // TODO-02 (continued): Return a project that sets nothing but the SDK and overrides Nullable
    //   to "disable" in its own body. The project body runs after Directory.Build.props, so it wins.
    public static string ProjectOverridingNullable() => throw new NotImplementedException();

    // TODO-03: Return a Directory.Build.targets that defines the property ProjectKind:
    //   "library" by default and "app" when OutputType is Exe. The condition must look at a value
    //   the project chose, which only works because .targets is imported AFTER the project body.
    public static string DirectoryBuildTargets() => throw new NotImplementedException();

    // TODO-04: Return the Directory.Build.props that lives in a nested "tests" folder. It must
    //   first import the parent file with
    //   $([MSBuild]::GetPathOfFileAbove('Directory.Build.props', '$(MSBuildThisFileDirectory)../'))
    //   and then set IsTestProject to true. Without the import the root file is hidden.
    public static string NestedDirectoryBuildProps() => throw new NotImplementedException();

    // TODO-05: Return a project (SDK, net10.0) with a custom target named WriteStamp. It must write
    //   one line "$(AssemblyName)|$(TargetFramework)" to the file "stamp.txt" inside
    //   $(MSBuildProjectDirectory), using the WriteLinesToFile task with Overwrite="true".
    public static string CustomTargetProject() => throw new NotImplementedException();

    // TODO-06 (optional): Return a project (SDK, net10.0) whose PropertyGroup has the condition
    //   '$(Configuration)' == 'Release' and sets TreatWarningsAsErrors to true.
    public static string ReleaseStrictProject() => throw new NotImplementedException();

    // TODO-07 (optional): Return a project (SDK, net10.0) that turns the implicit Compile glob off
    //   (EnableDefaultCompileItems false) and includes only "Program.cs" explicitly.
    public static string ExplicitCompileProject() => throw new NotImplementedException();
}
