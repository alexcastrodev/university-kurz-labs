namespace PartialLab;

public partial class Greeter(string prefix)
{
    private string Prefix => prefix;

    // Defining declaration: the body is in Greeter.Generated.cs.
    public partial string Greet(string who);

    public string GreetAll(IEnumerable<string> names) => string.Join(" ", names.Select(Greet));
}
