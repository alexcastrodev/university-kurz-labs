namespace PatternLab;

public record Address(string City, string Country);

public record Customer(string Name, Address? Address, int Orders);

public record Point(int X, int Y);

public enum State { Closed, Open, Locked }

public enum Trigger { Open, Close, Lock, Unlock }
