namespace GeneratedValuesLab;

public class Account
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Status { get; set; }
    public int Credits { get; set; } = -1;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? FullName { get; set; }
    public string? Initials { get; set; }
}

public class Ticket
{
    public int Id { get; set; }
    public Guid Ref { get; set; }
}
