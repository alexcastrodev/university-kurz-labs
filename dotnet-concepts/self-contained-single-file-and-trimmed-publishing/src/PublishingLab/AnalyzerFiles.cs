namespace PublishingLab;

/// <summary>
/// The analyzer half of the lab. The tests build a plain framework-dependent project (no
/// RuntimeIdentifier, so nothing is downloaded) and read the IL warnings from the build output.
/// </summary>
public static class AnalyzerFiles
{
    // TODO-04: Return a framework-dependent console project (net10.0, ImplicitUsings and Nullable enabled)
    //          on which the compiler runs BOTH the trim analyzer and the single-file analyzer, so that
    //          building SampleCode.RiskyProgram reports IL2026, IL2057, IL2070 and IL3000.
    //          Hint: PublishTrimmed and PublishSingleFile switch the analyzers on, or set
    //          EnableTrimAnalyzer and EnableSingleFileAnalyzer directly.
    public static string AnalyzerProject() =>
        throw new NotImplementedException("TODO-04");

    // TODO-05: Return the RiskyProgram rewritten so the analyzers report NO IL warning and it prints the
    //          same four lines (type=, methods=, json=, base=). Fixes to apply:
    //            - Type.GetType(name)          -> typeof(Plugin)
    //            - Type parameter t            -> [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
    //            - JsonSerializer.Serialize(x) -> a source-generated JsonSerializerContext
    //            - Assembly.Location           -> AppContext.BaseDirectory
    //          Print the base line as Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory).
    public static string FixedProgram() =>
        throw new NotImplementedException("TODO-05");

    // TODO-07 (optional): Return the same project as AnalyzerProject, plus WarningsAsErrors for
    //          IL2026, IL2057 and IL2070, so building the risky program FAILS instead of warning.
    public static string StrictAnalyzerProject() =>
        throw new NotImplementedException("TODO-07");
}
