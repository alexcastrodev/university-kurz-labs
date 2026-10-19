using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace EfCoreAdoNetDapperAndNosql;

/// <summary>A tiny key/document store on one table (Key, Json): flexible shape, no joins, no constraints.</summary>
public sealed class DocumentStore(SqliteConnection conn)
{
    // TODO-06 (bonus): insert or replace the document stored under key
    // (INSERT ... ON CONFLICT(Key) DO UPDATE, document serialized with JsonSerializer).
    public void Put<T>(string key, T document) => throw new NotImplementedException();

    // TODO-06 (bonus): read it back, or default when the key does not exist.
    public T? Get<T>(string key) => throw new NotImplementedException();

    // TODO-06 (bonus): keys (ordered) of documents whose JSON path equals value, e.g. ("$.color", "red").
    // SQLite: json_extract(Json, $path). Pass the path and the value as parameters.
    public List<string> FindKeys(string jsonPath, string value) => throw new NotImplementedException();
}
