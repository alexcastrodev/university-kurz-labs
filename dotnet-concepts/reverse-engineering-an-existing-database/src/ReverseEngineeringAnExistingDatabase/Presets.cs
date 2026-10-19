namespace ReverseEngineeringAnExistingDatabase;

/// <summary>Each preset is one flag (or two) of <c>dotnet ef dbcontext scaffold</c>.</summary>
public static class Presets
{
    /// <summary>TODO-01: <c>--use-database-names --no-pluralize</c>, so CUSTOMER stays CUSTOMER.</summary>
    public static ScaffoldSettings DatabaseNames()
        => throw new NotImplementedException("TODO-01");

    /// <summary>TODO-02: <c>--data-annotations</c>, attributes on the classes instead of Fluent calls.</summary>
    public static ScaffoldSettings DataAnnotations()
        => throw new NotImplementedException("TODO-02");

    /// <summary>TODO-03: one <c>--table</c> per name, only those tables (or views) are scaffolded.</summary>
    public static ScaffoldSettings OnlyTables(params string[] tables)
        => throw new NotImplementedException("TODO-03");
}
