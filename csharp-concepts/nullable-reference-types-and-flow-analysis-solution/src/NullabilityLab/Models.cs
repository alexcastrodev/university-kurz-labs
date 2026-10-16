using System.Diagnostics.CodeAnalysis;

namespace NullabilityLab;

public class User
{
    public required string Email { get; init; }

    public string? Nickname { get; init; }
}

public class Connection
{
    private string _endpoint;

    public Connection() => Initialize();

    public string Endpoint => _endpoint;

    [MemberNotNull(nameof(_endpoint))]
    private void Initialize() => _endpoint = "localhost:5432";
}
