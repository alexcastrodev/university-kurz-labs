namespace InterfacesLab;

public sealed class Person(string name, int age) : IComparable<Person>, IEquatable<Person>
{
    public string Name { get; } = name;
    public int Age { get; } = age;

    public int CompareTo(Person? other)
    {
        // TODO-00: Order by Age, then by Name with string.CompareOrdinal. Any instance sorts
        //   after null.
        throw new NotImplementedException("Not implemented yet.");
    }

    public bool Equals(Person? other)
    {
        // TODO-03: Two people are equal when Age matches and Name matches ignoring case.
        //   Also override Equals(object?) and GetHashCode so they agree with this method:
        //   equal people must produce the same hash code (use the same case-insensitive rule).
        throw new NotImplementedException("Not implemented yet.");
    }
}

public sealed class PersonByNameComparer : IComparer<Person>
{
    public int Compare(Person? x, Person? y)
    {
        // TODO-01: Compare by Name ignoring case. Handle null: null sorts first and two
        //   nulls are equal.
        throw new NotImplementedException("Not implemented yet.");
    }
}
