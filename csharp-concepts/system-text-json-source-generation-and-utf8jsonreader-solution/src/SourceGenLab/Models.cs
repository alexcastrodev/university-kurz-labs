namespace SourceGenLab;

public enum Status { Pending, Shipped }

public record Order(int Id, string Customer, decimal Total, string[] Tags, Status Status);

// Deliberately NOT registered in the context by the lab: it is the "forgotten type".
public record Person(string Name);
