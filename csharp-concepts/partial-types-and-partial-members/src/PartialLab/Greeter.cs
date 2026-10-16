namespace PartialLab;

public partial class Greeter(string prefix)
{
    // TODO-04: The generated half (Greeter.Generated.cs) reads Prefix. Expose the primary
    //   constructor parameter here, the way a LoggerMessage part uses your `logger`.
    private string Prefix => throw new NotImplementedException("Not implemented yet.");

    // Defining declaration: the body is in Greeter.Generated.cs.
    public partial string Greet(string who);

    public string GreetAll(IEnumerable<string> names)
    {
        // TODO-04: Join the greetings of all names with a single space, using Greet.
        throw new NotImplementedException("Not implemented yet.");
    }
}
