using Dapper;
using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>Dapper: you write the SQL, the mapping to objects is automatic.</summary>
public static class DapperProducts
{
    static DapperProducts()
    {
        // TODO-02: let the unit_price column map to the UnitPrice property.
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    // TODO-02: same query as AdoNetProducts.FindByCategory, now with Dapper.
    public static List<Product> FindByCategory(SqliteConnection conn, long categoryId, double minPrice) =>
        conn.Query<Product>(
            "SELECT Id, CategoryId, Name, unit_price FROM Products " +
            "WHERE CategoryId = @categoryId AND unit_price >= @minPrice ORDER BY unit_price",
            new { categoryId, minPrice }).ToList();

    // TODO-03: every category (also the empty one) with its products, in ONE query (no N+1).
    public static List<Category> CategoriesWithProducts(SqliteConnection conn)
    {
        var byId = new Dictionary<long, Category>();
        conn.Query<Category, Product?, Category>(
            "SELECT c.Id, c.Name, p.Id, p.CategoryId, p.Name, p.unit_price " +
            "FROM Categories c LEFT JOIN Products p ON p.CategoryId = c.Id ORDER BY c.Id, p.unit_price",
            (category, product) =>
            {
                if (!byId.TryGetValue(category.Id, out var known))
                    byId[category.Id] = known = category;
                if (product is not null)
                    known.Products.Add(product);
                return known;
            },
            splitOn: "Id");
        return byId.Values.OrderBy(c => c.Id).ToList();
    }

    // TODO-07 (bonus): insert many products with ONE Execute call and return the affected rows.
    public static int AddAll(SqliteConnection conn, IEnumerable<Product> products) =>
        conn.Execute(
            "INSERT INTO Products (CategoryId, Name, unit_price) VALUES (@CategoryId, @Name, @UnitPrice)",
            products);
}
