using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Dapper;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>EF Core: you write LINQ, EF Core writes the SQL and tracks the entities.</summary>
public static class EfProducts
{
    // TODO-04: same query as the other two, read-only: the context must not track the results.
    public static List<Product> ListReadOnly(AppDbContext db, long categoryId, double minPrice) =>
        throw new NotImplementedException();

    // TODO-05: multiply the prices of a category with Dapper INSIDE an EF Core transaction.
    // Share db.Database.GetDbConnection() and pass tx.GetDbTransaction() to Dapper.
    // commit == false must roll everything back. Returns the number of rows Dapper updated.
    public static int RaisePrices(AppDbContext db, long categoryId, double factor, bool commit) =>
        throw new NotImplementedException();
}
