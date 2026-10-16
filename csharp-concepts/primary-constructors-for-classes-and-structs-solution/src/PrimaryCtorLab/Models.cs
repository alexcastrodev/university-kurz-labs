namespace PrimaryCtorLab;

public interface IRepo
{
    string Load();
}

public class Account(string owner)
{
    public string Owner { get; private set; } = owner;

    public string Describe() => $"owner is {Owner}";

    public void Rename(string newOwner) => Owner = newOwner;
}

public class OrderService(IRepo repo)
{
    private readonly IRepo _repo = repo ?? throw new ArgumentNullException(nameof(repo));

    public string Load() => _repo.Load();
}

public struct Counter(int start)
{
    public Counter() : this(1) { }

    public int Value => start;

    public void Increment() => start++;
}

public class BankAccount(string id, string owner)
{
    public string Id { get; } = id;
    public string Owner { get; set; } = owner;
}

public class Savings(string id, string owner) : BankAccount(id, owner)
{
    public override string ToString() => $"{Id}/{Owner}";
}

public record Coordinates(int X, int Y)
{
    public int Sum => X + Y;
}

public class Service(string name)
{
    private readonly string name = name.Trim();

    public string Show() => name;
}
