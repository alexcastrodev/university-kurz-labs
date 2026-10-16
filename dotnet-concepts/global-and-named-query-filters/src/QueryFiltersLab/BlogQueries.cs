using Microsoft.EntityFrameworkCore;

namespace QueryFiltersLab;

public static class BlogQueries
{
    public static List<Blog> Visible(AppDbContext db) =>
        db.Blogs.OrderBy(b => b.Id).ToList();

    /// Soft-deleted blogs of the current tenant too (admin screen).
    public static List<Blog> IncludingDeleted(AppDbContext db) =>
        db.Blogs.OrderBy(b => b.Id).ToList(); // TODO-02: turn off only the soft-delete filter.

    /// Every tenant's non-deleted blogs (support tooling).
    public static List<Blog> AcrossTenants(AppDbContext db) =>
        db.Blogs.OrderBy(b => b.Id).ToList(); // TODO-03: turn off only the tenant filter.

    /// Undelete a blog. Another tenant's blog must stay out of reach.
    public static bool Undelete(AppDbContext db, int id)
    {
        // TODO-04: Find the blog with only the soft-delete filter off (the tenant filter must stay on).
        var blog = db.Blogs.SingleOrDefault(b => b.Id == id);
        if (blog is null) return false;
        blog.IsDeleted = false;
        db.SaveChanges();
        return true;
    }
}
