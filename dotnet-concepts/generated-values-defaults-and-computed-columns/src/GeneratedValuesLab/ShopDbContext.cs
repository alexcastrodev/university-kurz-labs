using Microsoft.EntityFrameworkCore;

namespace GeneratedValuesLab;

public class ShopDbContext(DbContextOptions<ShopDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(account =>
        {
            // TODO-00: Status has the database default "Active" (it is null in C# until saved).
            // TODO-01: Credits defaults to 10 in the database, but the C# initializer is -1 and
            //          an intentional 0 must be stored as 0. Which value must be the sentinel?
            // TODO-02: CreatedAt is filled by the database clock (CURRENT_TIMESTAMP).
            // TODO-03: FullName is a stored computed column: FirstName || ' ' || LastName.
            // TODO-04: UpdatedAt is set by the trigger in ShopDatabase on every update. Tell EF Core
            //          it is generated on add or update, then make sure the new value is read back
            //          after the trigger ran (SQLite RETURNING reports values before triggers).

            // TODO-06 (optional): Initials is a non-stored computed column built with
            //          upper(substr(FirstName, 1, 1)) || upper(substr(LastName, 1, 1)).
        });

        // TODO-05 (optional): Ticket.Ref is not a key, so no convention generates it. Attach a
        //          client-side value generator (GuidValueGenerator) so each ticket gets a new Guid.
    }
}
