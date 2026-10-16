namespace RecordLab;

public record Address(string Street, string City);

public record Customer(string Name, Address Address, IReadOnlyList<string> Tags);

public record Money(decimal Amount, string Currency)
{
    // TODO-02: Make the record reject invalid values even when it is built with a `with`
    //   expression. `with` does not call a constructor, it calls the init accessors, so the
    //   validation must live there. Declare Amount explicitly with an init accessor (the `field` keyword gives you
    //   the backing field) that throws ArgumentOutOfRangeException for a negative amount.
    //   An explicit property replaces the generated one, so it also needs an initializer
    //   (`= Amount`) for the constructor path, which does not go through the accessor.
}
