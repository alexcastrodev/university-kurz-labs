using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>Plain ADO.NET: you write the SQL, the parameters and the mapping.</summary>
public static class AdoNetProducts
{
    // TODO-00: products of a category with unit_price >= minPrice, ordered by price.
    public static List<Product> FindByCategory(SqliteConnection conn, long categoryId, double minPrice)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, CategoryId, Name, unit_price FROM Products " +
                          "WHERE CategoryId = $categoryId AND unit_price >= $minPrice ORDER BY unit_price";
        cmd.Parameters.AddWithValue("$categoryId", categoryId);
        cmd.Parameters.AddWithValue("$minPrice", minPrice);

        var result = new List<Product>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Product
            {
                Id = reader.GetInt64(reader.GetOrdinal("Id")),
                CategoryId = reader.GetInt64(reader.GetOrdinal("CategoryId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                UnitPrice = reader.GetDouble(reader.GetOrdinal("unit_price"))
            });
        }
        return result;
    }

    // TODO-00: insert one product and return the generated id. Parameters only, never concatenation.
    public static long Add(SqliteConnection conn, long categoryId, string name, double price, SqliteTransaction? tx = null)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = "INSERT INTO Products (CategoryId, Name, unit_price) VALUES ($c, $n, $p) RETURNING Id";
        cmd.Parameters.AddWithValue("$c", categoryId);
        cmd.Parameters.AddWithValue("$n", name);
        cmd.Parameters.AddWithValue("$p", price);
        return (long)cmd.ExecuteScalar()!;
    }

    // TODO-01: insert all products in one transaction: either all of them are stored or none.
    public static void AddAll(SqliteConnection conn, IEnumerable<Product> products)
    {
        using var tx = conn.BeginTransaction();
        foreach (var p in products)
            Add(conn, p.CategoryId, p.Name, p.UnitPrice, tx);
        tx.Commit(); // leaving the scope without Commit rolls back
    }
}
