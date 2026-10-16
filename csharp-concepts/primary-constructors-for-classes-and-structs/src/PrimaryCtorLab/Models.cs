namespace PrimaryCtorLab;

public interface IRepo
{
    string Load();
}

// TODO-00: The parameter `owner` is stored twice: once in Owner and once in a hidden mutable
//   field the compiler creates because Describe() and Rename() use it (warning CS9124).
//   Keep a single source of truth: make Owner `{ get; private set; }`, assign it in Rename()
//   and read Owner (not the parameter) in Describe().
public class Account(string owner)
{
    public string Owner { get; } = owner;

    public string Describe() => $"owner is {owner}";

    public void Rename(string newOwner) => owner = newOwner;
}

// TODO-01: A captured parameter is never validated and is mutable. Store it in a readonly
//   field `_repo` initialized from the parameter, throw ArgumentNullException(nameof(repo))
//   in that initializer when it is null, and use `_repo` in Load().
public class OrderService(IRepo repo)
{
    public string Load() => repo.Load();
}

// TODO-02: new Counter().Value must be 1 (today it is 0, the zeroed parameter). Declare an
//   explicit parameterless constructor that chains to the primary one with `this(1)`.
public struct Counter(int start)
{
    public int Value => start;

    public void Increment() => start++;
}

public class BankAccount(string id, string owner)
{
    public string Id { get; } = id;
    public string Owner { get; set; } = owner;
}

// TODO-03: `id` and `owner` are passed to the base AND captured here (warning CS9107), so
//   ToString() goes stale when Owner changes. Use the inherited Id and Owner properties in
//   ToString() instead of the parameters.
public class Savings(string id, string owner) : BankAccount(id, owner)
{
    public override string ToString() => $"{id}/{owner}";
}

// TODO-04 (optional): The parameters here are the public data of the type. Turn this class
//   into a positional record `Coordinates(int X, int Y)` so you get X, Y and value equality.
public class Coordinates(int x, int y)
{
    public int Sum => x + y;
}

// TODO-05 (optional): Declare `private readonly string name = name.Trim();` so the member sees
//   a readonly field (it shadows the parameter) and Show() returns the trimmed text.
public class Service(string name)
{
    public string Show() => name;
}
