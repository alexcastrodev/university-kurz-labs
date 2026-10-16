using Microsoft.EntityFrameworkCore;

namespace EfLogLab;

public static class CatalogQueries
{
    // TODO-01: Return the SQL (with the parameter header) of "products priced above min, ordered by name"
    // WITHOUT executing it. The query must capture the variable `min`, so it becomes the parameter @min.
    public static string ExpensiveSql(CatalogDbContext db, double min) =>
        throw new NotImplementedException("Not implemented yet.");

    // TODO-02: Return the same query as an IQueryable, tagged with "Catalog: expensive products" and
    // then with "Called from the price list screen". Do not execute it.
    public static IQueryable<Product> Expensive(CatalogDbContext db, double min) =>
        throw new NotImplementedException("Not implemented yet.");

    // Already working: the classic N+1, one command for the ids plus one per row.
    public static int LoadOneByOne(CatalogDbContext db)
    {
        var ids = db.Products.Select(p => p.Id).ToList();
        foreach (var id in ids) _ = db.Products.Single(p => p.Id == id);
        return ids.Count;
    }
}
