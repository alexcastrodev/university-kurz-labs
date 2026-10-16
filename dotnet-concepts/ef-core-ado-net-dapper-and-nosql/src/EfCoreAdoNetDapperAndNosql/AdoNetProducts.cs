using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>Plain ADO.NET: you write the SQL, the parameters and the mapping.</summary>
public static class AdoNetProducts
{
    // TODO-00: products of a category with unit_price >= minPrice, ordered by price.
    // Use a parameterized SqliteCommand and map each row of the SqliteDataReader by hand.
    public static List<Product> FindByCategory(SqliteConnection conn, long categoryId, double minPrice) =>
        throw new NotImplementedException();

    // TODO-00: insert one product and return the generated id (INSERT ... RETURNING Id).
    // Parameters only, never string concatenation. Set cmd.Transaction when tx is given.
    public static long Add(SqliteConnection conn, long categoryId, string name, double price, SqliteTransaction? tx = null) =>
        throw new NotImplementedException();

    // TODO-01: insert all products in one transaction: either all of them are stored or none.
    public static void AddAll(SqliteConnection conn, IEnumerable<Product> products) =>
        throw new NotImplementedException();
}
