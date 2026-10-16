using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>Creates and seeds the tables that all three data-access styles share (already done).</summary>
public static class Schema
{
    public static void CreateAndSeed(SqliteConnection conn)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE Categories (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL);
            CREATE TABLE Products (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryId INTEGER NOT NULL REFERENCES Categories(Id),
                Name TEXT NOT NULL,
                unit_price REAL NOT NULL CHECK (unit_price >= 0));
            CREATE TABLE Documents (Key TEXT PRIMARY KEY, Json TEXT NOT NULL);
            INSERT INTO Categories (Id, Name) VALUES (1, 'Books'), (2, 'Games'), (3, 'Empty');
            INSERT INTO Products (CategoryId, Name, unit_price) VALUES
                (1, 'Book A', 10), (1, 'Book B', 25), (1, 'Book C', 40),
                (2, 'Game A', 60), (2, 'Game B', 30);
            """;
        cmd.ExecuteNonQuery();
    }
}
