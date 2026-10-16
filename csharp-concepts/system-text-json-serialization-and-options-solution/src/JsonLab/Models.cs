using System.Text.Json.Serialization;

namespace JsonLab;

public enum Status { Pending, Shipped }

public record Order(int Id, string Customer, decimal Total, string? Note, Status Status);

public record Person(string Name, int Age);

public record Ledger(long Id, decimal Amount);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "kind")]
[JsonDerivedType(typeof(Circle), "circle")]
[JsonDerivedType(typeof(Square), "square")]
public abstract record Shape;

public record Circle(double Radius) : Shape;

public record Square(double Side) : Shape;
