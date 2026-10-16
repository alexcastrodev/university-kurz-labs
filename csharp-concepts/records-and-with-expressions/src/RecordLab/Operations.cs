namespace RecordLab;

public static class Operations
{
    public static Customer MoveTo(Customer customer, string city)
    {
        // TODO-00: Return a copy of the customer living in the new city, using nested `with`
        //   expressions. The original customer and its Address must stay untouched.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Customer AddTag(Customer customer, string tag)
    {
        // TODO-01: Return a copy with the tag appended. `with` is a shallow copy: if you reuse
        //   customer.Tags and mutate it, the original changes too. Build a new list instead.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static Money Add(Money first, Money second)
    {
        // TODO-03: Return a Money with the summed amount, built from `first` with a `with`
        //   expression. Throw InvalidOperationException when the currencies differ.
        throw new NotImplementedException("Not implemented yet.");
    }

    public static bool SameContent(Customer first, Customer second)
    {
        // TODO-04: Record equality compares the Tags list by reference, so two customers with
        //   equal tags are not Equal. Compare Name, Address and the tags element by element.
        throw new NotImplementedException("Not implemented yet.");
    }
}
