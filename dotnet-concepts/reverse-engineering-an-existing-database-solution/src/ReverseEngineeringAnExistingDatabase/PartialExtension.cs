namespace ReverseEngineeringAnExistingDatabase;

/// <summary>
/// Hand-written code that survives <c>--force</c>: a second file that completes the scaffolded
/// partial classes. Generated files are never edited.
/// </summary>
public static class PartialExtension
{
    /// <summary>
    /// TODO-05: source text of a hand-written file in namespace <paramref name="ns"/> that
    /// (1) completes <c>partial class LegacyDb</c> with <c>OnModelCreatingPartial</c>, making
    /// <c>OrderLine</c> to <c>OrderHdr</c> cascade on delete, and
    /// (2) adds a computed property <c>Display</c> (<c>"{CustName} ({Status})"</c>) to <c>partial class Customer</c>.
    /// </summary>
    public static string Source(string ns) => $$"""
        using Microsoft.EntityFrameworkCore;

        namespace {{ns}};

        public partial class LegacyDb
        {
            partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<OrderLine>()
                    .HasOne(l => l.Ord).WithMany(o => o.OrderLines)
                    .HasForeignKey(l => l.OrdId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }

        public partial class Customer
        {
            public string Display => $"{CustName} ({Status})";
        }
        """;

    /// <summary>
    /// TODO-07 (optional): the same file, plus the key fix from the Trade-offs section:
    /// <c>Customer.CustId</c> must be <c>ValueGeneratedOnAdd()</c>, because the column is a SQLite row id alias
    /// and the scaffolder guessed <c>ValueGeneratedNever()</c>. Keep the OrderLine cascade from <see cref="Source"/>.
    /// </summary>
    public static string SourceWithGeneratedKeys(string ns) => $$"""
        using Microsoft.EntityFrameworkCore;

        namespace {{ns}};

        public partial class LegacyDb
        {
            partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<OrderLine>()
                    .HasOne(l => l.Ord).WithMany(o => o.OrderLines)
                    .HasForeignKey(l => l.OrdId)
                    .OnDelete(DeleteBehavior.Cascade);
                modelBuilder.Entity<Customer>().Property(c => c.CustId).ValueGeneratedOnAdd();
            }
        }

        public partial class Customer
        {
            public string Display => $"{CustName} ({Status})";
        }
        """;
}
