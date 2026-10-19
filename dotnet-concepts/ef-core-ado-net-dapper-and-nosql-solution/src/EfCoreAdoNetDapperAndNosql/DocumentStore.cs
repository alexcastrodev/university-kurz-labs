using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>A tiny key/document store on one table (Key, Json): flexible shape, no joins, no constraints.</summary>
public sealed class DocumentStore(SqliteConnection conn)
{
    // TODO-06 (bonus): insert or replace the document stored under key.
    public void Put<T>(string key, T document)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Documents (Key, Json) VALUES ($k, $j) " +
                          "ON CONFLICT(Key) DO UPDATE SET Json = excluded.Json";
        cmd.Parameters.AddWithValue("$k", key);
        cmd.Parameters.AddWithValue("$j", JsonSerializer.Serialize(document));
        cmd.ExecuteNonQuery();
    }

    // TODO-06 (bonus): read it back, or default when the key does not exist.
    public T? Get<T>(string key)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Json FROM Documents WHERE Key = $k";
        cmd.Parameters.AddWithValue("$k", key);
        return cmd.ExecuteScalar() is string json ? JsonSerializer.Deserialize<T>(json) : default;
    }

    // TODO-06 (bonus): keys (ordered) of documents whose JSON path equals value, e.g. ("$.color", "red").
    public List<string> FindKeys(string jsonPath, string value)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Key FROM Documents WHERE json_extract(Json, $path) = $v ORDER BY Key";
        cmd.Parameters.AddWithValue("$path", jsonPath);
        cmd.Parameters.AddWithValue("$v", value);
        var keys = new List<string>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read()) keys.Add(reader.GetString(0));
        return keys;
    }
}
