namespace InterfacesLab;

public sealed class Person(string name, int age) : IComparable<Person>, IEquatable<Person>
{
    public string Name { get; } = name;
    public int Age { get; } = age;

    public int CompareTo(Person? other)
    {
        if (other is null) return 1;
        var byAge = Age.CompareTo(other.Age);
        return byAge != 0 ? byAge : string.CompareOrdinal(Name, other.Name);
    }

    public bool Equals(Person? other) =>
        other is not null && Age == other.Age && string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => Equals(obj as Person);

    public override int GetHashCode() => HashCode.Combine(StringComparer.OrdinalIgnoreCase.GetHashCode(Name), Age);
}

public sealed class PersonByNameComparer : IComparer<Person>
{
    public int Compare(Person? x, Person? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        return string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }
}
