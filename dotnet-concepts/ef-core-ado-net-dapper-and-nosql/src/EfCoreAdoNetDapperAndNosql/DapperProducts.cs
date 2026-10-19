using Dapper;
using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>Dapper: you write the SQL, the mapping to objects is automatic.</summary>
public static class DapperProducts
{
    static DapperProducts()
    {
        // TODO-02: let the unit_price column map to the UnitPrice property
        // (DefaultTypeMap.MatchNamesWithUnderscores, or an AS alias in every SELECT).
    }

    // TODO-02: same query as AdoNetProducts.FindByCategory, now with Dapper (Query<T> and an anonymous parameter object).
    public static List<Product> FindByCategory(SqliteConnection conn, long categoryId, double minPrice) =>
        throw new NotImplementedException();

    // TODO-03: every category (also the empty one) with its products, in ONE query (no N+1).
    // Hint: LEFT JOIN, Query<Category, Product?, Category> with splitOn, and a dictionary to reuse the Category.
    public static List<Category> CategoriesWithProducts(SqliteConnection conn) =>
        throw new NotImplementedException();

    // TODO-07 (bonus): insert many products with ONE Execute call (pass the list) and return the affected rows.
    public static int AddAll(SqliteConnection conn, IEnumerable<Product> products) =>
        throw new NotImplementedException();
}
