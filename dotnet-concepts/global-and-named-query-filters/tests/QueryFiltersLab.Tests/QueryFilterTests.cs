using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace QueryFiltersLab.Tests;

public sealed class QueryFilterTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly DbContextOptions<AppDbContext> _options;

    public QueryFilterTests()
    {
        _connection.Open();
        _options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(_connection).Options;

        using var db = Db("t1");
        db.Database.EnsureCreated();
        db.Blogs.AddRange(
            new Blog { Id = 1, Name = "fish-t1", TenantId = "t1" },
            new Blog { Id = 2, Name = "fish-t1-deleted", TenantId = "t1", IsDeleted = true },
            new Blog { Id = 3, Name = "cats-t2", TenantId = "t2" },
            new Blog { Id = 4, Name = "cats-t2-deleted", TenantId = "t2", IsDeleted = true });
        db.Posts.AddRange(
            new Post { Id = 1, Title = "visible" },
            new Post { Id = 2, Title = "gone", IsDeleted = true });
        db.SaveChanges();
    }

    private AppDbContext Db(string tenant) => new(_options, tenant);

    private static string[] Names(IEnumerable<Blog> blogs) => blogs.Select(b => b.Name).ToArray();

    public void Dispose() => _connection.Dispose();

    [Fact]
    public void ShouldHideDeletedBlogsAndOtherTenantsByDefault()
    {
        using var db = Db("t1");

        Assert.Equal(["fish-t1"], Names(BlogQueries.Visible(db)));
    }

    [Fact]
    public void ShouldUseTheTenantOfEachContextInstance()
    {
        using var t1 = Db("t1");
        using var t2 = Db("t2");

        Assert.Equal(["fish-t1"], Names(BlogQueries.Visible(t1)));
        Assert.Equal(["cats-t2"], Names(BlogQueries.Visible(t2)));
    }

    [Fact]
    public void ShouldApplySoftDeleteFilterToEveryEntityThatHasTheFlag()
    {
        using var db = Db("t1");

        Assert.Equal(["visible"], db.Posts.Select(p => p.Title).ToArray());
    }

    [Fact]
    public void ShouldDeclareTwoNamedFiltersOnBlogAndOneOnPost()
    {
        using var db = Db("t1");

        var blogFilters = db.Model.FindEntityType(typeof(Blog))!.GetDeclaredQueryFilters().Select(f => f.Key);
        var postFilters = db.Model.FindEntityType(typeof(Post))!.GetDeclaredQueryFilters().Select(f => f.Key);

        Assert.Equal(
            [AppDbContext.SoftDeletionFilter, AppDbContext.TenantFilter],
            blogFilters.Order().ToArray());
        Assert.Equal([AppDbContext.SoftDeletionFilter], postFilters.ToArray());
    }

    [Fact]
    public void ShouldIncludeDeletedBlogsOfCurrentTenantOnly()
    {
        using var db = Db("t1");

        Assert.Equal(["fish-t1", "fish-t1-deleted"], Names(BlogQueries.IncludingDeleted(db)));
    }

    [Fact]
    public void ShouldIncludeOtherTenantsButStillHideDeleted()
    {
        using var db = Db("t1");

        Assert.Equal(["fish-t1", "cats-t2"], Names(BlogQueries.AcrossTenants(db)));
    }

    [Fact]
    public void ShouldUndeleteBlogOfCurrentTenant()
    {
        using (var db = Db("t1"))
            Assert.True(BlogQueries.Undelete(db, 2));

        using var check = Db("t1");
        Assert.Equal(["fish-t1", "fish-t1-deleted"], Names(BlogQueries.Visible(check)));
    }

    [Fact]
    public void ShouldNotUndeleteBlogOfAnotherTenant()
    {
        using var db = Db("t1");

        Assert.False(BlogQueries.Undelete(db, 4));
        using var t2 = Db("t2");
        Assert.Equal(["cats-t2"], Names(BlogQueries.Visible(t2)));
    }
}
