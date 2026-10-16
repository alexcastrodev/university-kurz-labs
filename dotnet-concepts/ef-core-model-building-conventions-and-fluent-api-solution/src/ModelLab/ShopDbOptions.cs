using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ModelLab;

public static class ShopDbOptions
{
    public static DbContextOptions<ShopDbContext> Create(SqliteConnection connection) =>
        new DbContextOptionsBuilder<ShopDbContext>()
            .UseSqlite(connection)
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Category>().Any(c => c.Id == 3))
                {
                    context.Set<Category>().Add(new Category { Id = 3, Name = "Added at runtime", Slug = "runtime" });
                    context.SaveChanges();
                }
            })
            .Options;
}
