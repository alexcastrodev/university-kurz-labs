namespace LinqLab;

public record Customer(int Id, string Name, string Email);

public record Order(int Id, int? CustomerId, decimal Total);
