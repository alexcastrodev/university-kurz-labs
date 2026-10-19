using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Dapper;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>EF Core: you write LINQ, EF Core writes the SQL and tracks the entities.</summary>
public static class EfProducts
{
    // TODO-04: same query as the other two, read-only: the context must not track the results.
    public static List<Product> ListReadOnly(AppDbContext db, long categoryId, double minPrice) =>
        db.Products.AsNoTracking()
            .Where(p => p.CategoryId == categoryId && p.UnitPrice >= minPrice)
            .OrderBy(p => p.UnitPrice)
            .ToList();

    // TODO-05: multiply the prices of a category with Dapper INSIDE an EF Core transaction.
    // commit == false must roll everything back. Returns the number of rows Dapper updated.
    public static int RaisePrices(AppDbContext db, long categoryId, double factor, bool commit)
    {
        using var tx = db.Database.BeginTransaction();
        var conn = db.Database.GetDbConnection();
        var affected = conn.Execute(
            "UPDATE Products SET unit_price = unit_price * @factor WHERE CategoryId = @categoryId",
            new { factor, categoryId }, tx.GetDbTransaction());
        if (commit) tx.Commit(); else tx.Rollback();
        return affected;
    }
}
