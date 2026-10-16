using Microsoft.EntityFrameworkCore;

namespace GeneratedValuesLab;

public static class ShopDatabase
{
    public static void Create(ShopDbContext db)
    {
        db.Database.EnsureCreated();
        db.Database.ExecuteSqlRaw("""
            CREATE TRIGGER Accounts_SetUpdatedAt AFTER UPDATE ON Accounts
            FOR EACH ROW WHEN NEW.UpdatedAt IS OLD.UpdatedAt
            BEGIN
                UPDATE Accounts SET UpdatedAt = CURRENT_TIMESTAMP WHERE Id = NEW.Id;
            END;
            """);
    }
}
