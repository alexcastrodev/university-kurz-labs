using Microsoft.EntityFrameworkCore;

namespace EfLogLab;

public static class CatalogQueries
{
    public static string ExpensiveSql(CatalogDbContext db, double min) =>
        db.Products.Where(p => p.Price > min).OrderBy(p => p.Name).ToQueryString();

    public static IQueryable<Product> Expensive(CatalogDbContext db, double min) =>
        db.Products
            .TagWith("Catalog: expensive products")
            .TagWith("Called from the price list screen")
            .Where(p => p.Price > min)
            .OrderBy(p => p.Name);

    public static IQueryable<Product> ExpensiveWithCallSite(CatalogDbContext db, double min) =>
        Expensive(db, min).TagWithCallSite();

    // The classic N+1, one command for the ids plus one per row.
    public static int LoadOneByOne(CatalogDbContext db)
    {
        var ids = db.Products.Select(p => p.Id).ToList();
        foreach (var id in ids) _ = db.Products.Single(p => p.Id == id);
        return ids.Count;
    }
}
