namespace RecordLab;

public record Address(string Street, string City);

public record Customer(string Name, Address Address, IReadOnlyList<string> Tags);

public record Money(decimal Amount, string Currency)
{
    // The property initializer runs for the constructor, the init accessor runs for `with`.
    public decimal Amount { get; init => field = NotNegative(value); } = NotNegative(Amount);

    private static decimal NotNegative(decimal value) =>
        value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Amount), value, "Amount cannot be negative.");
}
