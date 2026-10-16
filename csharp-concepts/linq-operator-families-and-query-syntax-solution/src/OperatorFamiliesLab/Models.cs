namespace OperatorFamiliesLab;

public record User(int Id, string Email);

public record Product(string Name, int Stock, decimal Price);

public record Player(string Team, string Name, int Score);
