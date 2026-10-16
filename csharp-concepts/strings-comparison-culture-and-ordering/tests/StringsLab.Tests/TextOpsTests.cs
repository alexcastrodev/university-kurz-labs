using System.Globalization;

namespace StringsLab.Tests;

public class TextOpsTests
{
    private static readonly string[] Words = ["zebra", "Apple", "apple", "Zebra", "äpple", "Äpple"];

    [Fact]
    public void ShouldSortByCodeUnitWhenOrdinal()
    {
        Assert.Equal(["Apple", "Zebra", "apple", "zebra", "Äpple", "äpple"], TextOps.SortOrdinal(Words));
    }

    [Fact]
    public void ShouldKeepAccentedWordsNextToTheirBaseLettersInEnglish()
    {
        var sorted = TextOps.SortForPerson(Words, new CultureInfo("en-US"));

        Assert.Equal(["apple", "Apple", "äpple", "Äpple", "zebra", "Zebra"], sorted);
    }

    [Fact]
    public void ShouldPutAUmlautAfterZInSwedish()
    {
        var sorted = TextOps.SortForPerson(Words, new CultureInfo("sv-SE"));

        Assert.Equal(["apple", "Apple", "zebra", "Zebra", "äpple", "Äpple"], sorted);
    }

    [Fact]
    public void ShouldMatchIdentifiersIgnoringCaseOnly()
    {
        Assert.True(TextOps.SameIdentifier("Content-Type", "content-type"));
        Assert.False(TextOps.SameIdentifier("é", "é"));
    }

    [Fact]
    public void ShouldTreatCanonicallyEquivalentTextAsEqualForPeople()
    {
        Assert.True(TextOps.SameText("é", "é"));
        Assert.True(TextOps.SameText("ab", "a­b"));
        Assert.False(TextOps.SameText("abc", "abd"));
    }

    [Fact]
    public void ShouldLowerProtocolTextTheSameWayInEveryCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");

            Assert.Equal("title", TextOps.LowerProtocol("TITLE"));
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }
    }

    [Fact]
    public void ShouldSortNumberedNamesByTheirNumbers()
    {
        string[] files = ["file10.txt", "file2.txt", "file1.txt", "file07.txt", "file7.txt"];

        Assert.Equal(
            ["file1.txt", "file2.txt", "file07.txt", "file7.txt", "file10.txt"],
            TextOps.NaturalOrder(files));
    }

    [Fact]
    public void ShouldPutWindows8BeforeWindows10()
    {
        Assert.Equal(["Windows 8", "Windows 10"], TextOps.NaturalOrder(["Windows 10", "Windows 8"]));
    }

    [Fact]
    public void ShouldFindATermIgnoringAccentsAndCase()
    {
        Assert.True(TextOps.ContainsIgnoringAccents("Crème Brûlée", "creme brulee"));
        Assert.False(TextOps.ContainsIgnoringAccents("Crème Brûlée", "tiramisu"));
    }

    [Fact]
    public void ShouldReturnTheSameInstanceForEqualStrings()
    {
        var runtimeBuilt = new string(['h', 'e', 'l', 'l', 'o']);

        Assert.False(ReferenceEquals(runtimeBuilt, "hello"));
        Assert.True(ReferenceEquals(TextOps.Canonical(runtimeBuilt), "hello"));
    }
}
