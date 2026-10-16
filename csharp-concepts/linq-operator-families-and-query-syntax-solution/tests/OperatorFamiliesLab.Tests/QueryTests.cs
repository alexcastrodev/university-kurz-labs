using System.Globalization;

namespace OperatorFamiliesLab.Tests;

public class QueryTests
{
    private static readonly string[] Words = ["apple", "fig", "banana", "kiwi", "cherry"];

    // TODO-00
    [Fact]
    public void ShouldFindTheUniqueUserIgnoringCase()
    {
        User[] users = [new(1, "ana@x.com"), new(2, "bia@x.com")];

        Assert.Equal(2, Queries.FindByEmail(users, "BIA@x.com")?.Id);
    }

    [Fact]
    public void ShouldReturnNullWhenNoUserMatches()
    {
        User[] users = [new(1, "ana@x.com")];

        Assert.Null(Queries.FindByEmail(users, "zed@x.com"));
        Assert.Null(Queries.FindByEmail([], "zed@x.com"));
    }

    [Fact]
    public void ShouldFailWhenTheEmailIsNotUnique()
    {
        User[] users = [new(1, "ana@x.com"), new(2, "ANA@x.com")];

        Assert.Throws<InvalidOperationException>(() => Queries.FindByEmail(users, "ana@x.com"));
    }

    // TODO-01
    [Fact]
    public void ShouldNotCallAnEmptyShelfInStock()
    {
        Assert.False(Queries.AllInStock([]));
    }

    [Fact]
    public void ShouldCheckEveryProductForStock()
    {
        Assert.True(Queries.AllInStock([new("a", 1, 1m), new("b", 5, 2m)]));
        Assert.False(Queries.AllInStock([new("a", 1, 1m), new("b", 0, 2m)]));
    }

    [Fact]
    public void ShouldReturnTheMostExpensiveProductItself()
    {
        Product[] products = [new("a", 1, 9.5m), new("b", 1, 20m), new("c", 1, 3m)];

        Assert.Equal("b", Queries.MostExpensive(products)?.Name);
        Assert.Null(Queries.MostExpensive([]));
    }

    // TODO-02
    [Fact]
    public void ShouldRankByTeamThenScoreDescendingThenName()
    {
        Player[] players =
        [
            new("B", "Zed", 10), new("A", "Bob", 10), new("A", "Amy", 10),
            new("A", "Cy", 20), new("B", "Yan", 30),
        ];

        Assert.Equal(["Cy", "Amy", "Bob", "Yan", "Zed"], Queries.Ranking(players));
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("sv-SE")]
    [InlineData("")]
    public void ShouldSortKeysOrdinallyUnderAnyCulture(string culture)
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

            Assert.Equal(["A", "B", "a", "b"], Queries.SortedKeys(["b", "a", "B", "A"]));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    // TODO-03
    [Fact]
    public void ShouldReturnOneBasedPages()
    {
        int[] items = [1, 2, 3, 4, 5, 6, 7];

        Assert.Equal([1, 2, 3], Queries.Page(items, 1, 3));
        Assert.Equal([7], Queries.Page(items, 3, 3));
        Assert.Empty(Queries.Page(items, 4, 3));
    }

    [Fact]
    public void ShouldSumEachBatch()
    {
        Assert.Equal([6, 15, 7], Queries.BatchSums([1, 2, 3, 4, 5, 6, 7], 3));
        Assert.Empty(Queries.BatchSums([], 3));
    }

    // TODO-04
    [Fact]
    public void ShouldKeepOnlyBoxedInts()
    {
        object?[] mixed = [1, "two", 3, null, 4L];

        Assert.Equal([1, 3], Queries.IntsOnly(mixed));
    }

    [Fact]
    public void ShouldCastAllItemsOrFailLoudly()
    {
        Assert.Equal([1, 2], Queries.AllInts([1, 2]));
        Assert.Throws<InvalidCastException>(() => Queries.AllInts([1, "two", 3]));
        Assert.Throws<InvalidCastException>(() => Queries.AllInts([4L]));
    }

    // TODO-05
    [Fact]
    public void ShouldDescribeLongWordsLongestFirst()
    {
        Assert.Equal(["banana:6", "cherry:6", "apple:5", "kiwi:4"], Queries.Describe(Words));
    }

    [Fact]
    public void ShouldFindLengthsSharedByMoreThanOneWord()
    {
        Assert.Equal([6], Queries.RepeatedLengths(Words));
        Assert.Equal([3, 4], Queries.RepeatedLengths(["cat", "dog", "kiwi", "pear", "fig", "a"]));
    }
}
