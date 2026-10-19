namespace ReverseEngineeringAnExistingDatabase;

/// <summary>Each preset is one flag (or two) of <c>dotnet ef dbcontext scaffold</c>.</summary>
public static class Presets
{
    /// <summary>TODO-01: <c>--use-database-names --no-pluralize</c>, so CUSTOMER stays CUSTOMER.</summary>
    public static ScaffoldSettings DatabaseNames()
        => new(UseDatabaseNames: true, NoPluralize: true);

    /// <summary>TODO-02: <c>--data-annotations</c>, attributes on the classes instead of Fluent calls.</summary>
    public static ScaffoldSettings DataAnnotations()
        => new(UseDataAnnotations: true);

    /// <summary>TODO-03: one <c>--table</c> per name, only those tables (or views) are scaffolded.</summary>
    public static ScaffoldSettings OnlyTables(params string[] tables)
        => new(Tables: tables);
}
