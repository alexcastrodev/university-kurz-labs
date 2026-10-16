using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ModelLab;

public static class ShopDbOptions
{
    public static DbContextOptions<ShopDbContext> Create(SqliteConnection connection) =>
        new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            // TODO-08 (optional): UseSeeding that adds Category { Id = 3, Name = "Added at runtime", Slug = "runtime" }
            //          when it is missing. Compare with HasData: only HasData lands in a migration.
            .Options;
}
