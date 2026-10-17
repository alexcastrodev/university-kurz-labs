using Microsoft.Data.Sqlite;

namespace ReverseEngineeringAnExistingDatabase;

/// <summary>The legacy database the scaffolder reads (already done). Upper-case snake names on purpose.</summary>
public static class LegacySchema
{
    public const string Script = """
        CREATE TABLE CUSTOMER (cust_id INTEGER PRIMARY KEY, cust_name TEXT NOT NULL, email TEXT,
            status TEXT NOT NULL DEFAULT 'A', created TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);
        CREATE UNIQUE INDEX UX_CUSTOMER_EMAIL ON CUSTOMER (email);
        CREATE TABLE ORDER_HDR (ord_id INTEGER PRIMARY KEY, cust_id INTEGER NOT NULL REFERENCES CUSTOMER,
            total NUMERIC NOT NULL);
        CREATE TABLE ORDER_LINE (ord_id INTEGER NOT NULL REFERENCES ORDER_HDR, line_no INTEGER NOT NULL,
            sku TEXT NOT NULL, qty INTEGER NOT NULL, PRIMARY KEY (ord_id, line_no));
        CREATE TABLE TAG (tag_id INTEGER PRIMARY KEY, label TEXT NOT NULL);
        CREATE TABLE ORDER_TAG (ord_id INTEGER NOT NULL REFERENCES ORDER_HDR,
            tag_id INTEGER NOT NULL REFERENCES TAG, PRIMARY KEY (ord_id, tag_id));
        CREATE TABLE AUDIT_LOG (at TEXT, what TEXT);
        CREATE VIEW V_ORDER_TOTALS AS SELECT c.cust_id, c.cust_name, SUM(o.total) AS total_spent
            FROM CUSTOMER c JOIN ORDER_HDR o ON o.cust_id = c.cust_id GROUP BY c.cust_id, c.cust_name;
        """;

    /// <summary>Creates the legacy database file at <paramref name="path"/> and returns its connection string.</summary>
    public static string Create(string path)
    {
        var connectionString = $"Data Source={path};Pooling=False";
        using var conn = new SqliteConnection(connectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = Script;
        cmd.ExecuteNonQuery();
        return connectionString;
    }
}
