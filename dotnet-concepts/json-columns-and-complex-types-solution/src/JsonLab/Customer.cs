namespace JsonLab;

public record Address(string Street, string City, string Country);

public class Customer
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<string> Tags { get; set; } = [];
    public required Address Address { get; set; }
    public Address? Billing { get; set; }
    public List<Address> Shipping { get; set; } = [];
}
