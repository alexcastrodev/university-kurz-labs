namespace NullabilityLab;

public class User
{
    public required string Email { get; init; }

    // TODO-00: Nickname is optional. Declare it as nullable and remove the null! trick that
    //   silences the compiler. Then read the warnings the compiler gives you in DisplayName.
    public string Nickname { get; init; } = null!;
}

public class Connection
{
    // TODO-04: The compiler warns (CS8618) that _endpoint is not initialized by the constructor,
    //   even though Initialize sets it. Annotate Initialize with [MemberNotNull] so the warning
    //   disappears without changing the field type.
    private string _endpoint;

    public Connection() => Initialize();

    public string Endpoint => _endpoint;

    private void Initialize() => _endpoint = "localhost:5432";
}
