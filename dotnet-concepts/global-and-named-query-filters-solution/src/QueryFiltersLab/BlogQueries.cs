using Microsoft.EntityFrameworkCore;

namespace QueryFiltersLab;

public static class BlogQueries
{
    public static List<Blog> Visible(AppDbContext db) =>
        db.Blogs.OrderBy(b => b.Id).ToList();

    /// Soft-deleted blogs of the current tenant too (admin screen).
    public static List<Blog> IncludingDeleted(AppDbContext db) =>
        db.Blogs.IgnoreQueryFilters([AppDbContext.SoftDeletionFilter]).OrderBy(b => b.Id).ToList();

    /// Every tenant's non-deleted blogs (support tooling).
    public static List<Blog> AcrossTenants(AppDbContext db) =>
        db.Blogs.IgnoreQueryFilters([AppDbContext.TenantFilter]).OrderBy(b => b.Id).ToList();

    /// Undelete: only the soft-delete rule is off, so another tenant's blog stays out of reach.
    public static bool Undelete(AppDbContext db, int id)
    {
        var blog = db.Blogs.IgnoreQueryFilters([AppDbContext.SoftDeletionFilter]).SingleOrDefault(b => b.Id == id);
        if (blog is null) return false;
        blog.IsDeleted = false;
        db.SaveChanges();
        return true;
    }
}
