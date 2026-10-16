namespace InterfacesLab.Tests;

public class InterfacesTests
{
    [Fact]
    public void ShouldSortPeopleByAgeThenByName()
    {
        var people = new List<Person> { new("Zoe", 30), new("Ann", 30), new("Bob", 20) };

        people.Sort();

        Assert.Equal(["Bob", "Ann", "Zoe"], people.Select(p => p.Name));
    }

    [Fact]
    public void ShouldPlaceAnythingAfterNull()
    {
        Assert.True(new Person("Ann", 20).CompareTo(null) > 0);
    }

    [Fact]
    public void ShouldSortByNameIgnoringCaseWithAComparer()
    {
        var people = new List<Person> { new("bob", 20), new("Carl", 50), new("Ann", 70) };

        people.Sort(new PersonByNameComparer());

        Assert.Equal(["Ann", "bob", "Carl"], people.Select(p => p.Name));
    }

    [Fact]
    public void ShouldTreatSameNameIgnoringCaseAndSameAgeAsEqual()
    {
        Person ann = new("Ann", 30);

        Assert.True(ann.Equals(new Person("ANN", 30)));
        Assert.False(ann.Equals(new Person("Ann", 31)));
        Assert.False(ann.Equals((Person?)null));
    }

    [Fact]
    public void ShouldOverrideObjectEqualsAndGetHashCodeConsistently()
    {
        object ann = new Person("Ann", 30);
        var shout = new Person("ANN", 30);

        Assert.True(ann.Equals(shout));
        Assert.Equal(ann.GetHashCode(), shout.GetHashCode());
        Assert.Single(new HashSet<Person> { (Person)ann, shout });
    }

    [Fact]
    public void ShouldPickTheImplementationByInterface()
    {
        var cowboy = new Cowboy();

        Assert.Equal("draws a picture", ((IDrawable)cowboy).Draw());
        Assert.Equal("draws a gun", ((IWeapon)cowboy).Draw());
    }

    [Fact]
    public void ShouldNotExposeDrawOnTheClassItself()
    {
        Assert.Null(typeof(Cowboy).GetMethod("Draw"));
    }

    [Fact]
    public void ShouldUseTheDefaultMethodThroughTheInterface()
    {
        var logger = new ListLogger();

        ((ILogger)logger).Info("started");

        Assert.Equal(["INFO: started"], logger.Lines);
    }

    [Fact]
    public void ShouldNotInheritTheDefaultMethodInTheClass()
    {
        Assert.Null(typeof(ListLogger).GetMethod("Info"));
        Assert.NotNull(typeof(ILogger).GetMethod("Info")!.GetMethodBody());
    }

    [Fact]
    public void ShouldEnumerateThroughBothEnumerableInterfaces()
    {
        var playlist = new Playlist(["a", "b", "c"]);

        Assert.Equal(["a", "b", "c"], playlist.ToList());

        var nonGeneric = new List<object?>();
        var enumerator = ((System.Collections.IEnumerable)playlist).GetEnumerator();
        while (enumerator.MoveNext()) nonGeneric.Add(enumerator.Current);
        Assert.Equal(["a", "b", "c"], nonGeneric);
    }

    [Fact]
    public void ShouldKeepTheNonGenericGetEnumeratorOffThePublicSurface()
    {
        var publicEnumerators = typeof(Playlist).GetMethods().Where(m => m.Name == "GetEnumerator").ToList();

        Assert.Single(publicEnumerators);
        Assert.Equal(typeof(IEnumerator<string>), publicEnumerators[0].ReturnType);
    }

    // Bonus tests (TODO-06 and TODO-07 exist only in the solution).

    [Fact]
    public void ShouldReadAStaticAbstractMemberThroughTheTypeParameter()
    {
        Assert.Equal("5 m", Units.Describe<Meter>(5));
        Assert.Equal("2 kg", Units.Describe<Kilogram>(2));
    }

    [Fact]
    public void ShouldResolveCompetingDefaultMethodsInTheClass()
    {
        var both = new Both();

        Assert.Equal("left+right", ((ILeft)both).Name);
        Assert.Equal("right+left", ((IRight)both).Name);
    }
}
