namespace PartialLab;

// This file simulates the half a source generator would write. Do not edit it.
// It calls Prefix, which the hand-written part in Greeter.cs must provide.
public partial class Greeter
{
    public partial string Greet(string who) => $"{Prefix}{who}!";
}
