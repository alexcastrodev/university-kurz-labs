namespace RecordLab;

public static class Operations
{
    public static Customer MoveTo(Customer customer, string city) =>
        customer with { Address = customer.Address with { City = city } };

    public static Customer AddTag(Customer customer, string tag) =>
        customer with { Tags = [.. customer.Tags, tag] };

    public static Money Add(Money first, Money second) =>
        first.Currency == second.Currency
            ? first with { Amount = first.Amount + second.Amount }
            : throw new InvalidOperationException($"Cannot add {first.Currency} and {second.Currency}.");

    public static bool SameContent(Customer first, Customer second) =>
        first.Name == second.Name
        && first.Address == second.Address
        && first.Tags.SequenceEqual(second.Tags);
}
