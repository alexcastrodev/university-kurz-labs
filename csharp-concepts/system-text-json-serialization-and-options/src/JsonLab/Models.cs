namespace JsonLab;

public enum Status { Pending, Shipped }

public record Order(int Id, string Customer, decimal Total, string? Note, Status Status);

public record Person(string Name, int Age);

public record Ledger(long Id, decimal Amount);

// TODO-02: Make Shape round-trip its hierarchy. Declare Circle and Square as derived types
//   with the discriminator values "circle" and "square", and name the discriminator
//   property "kind" ([JsonPolymorphic] and [JsonDerivedType]).
public abstract record Shape;

public record Circle(double Radius) : Shape;

public record Square(double Side) : Shape;
