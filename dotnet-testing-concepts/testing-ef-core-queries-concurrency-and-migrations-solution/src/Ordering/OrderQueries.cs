using Microsoft.EntityFrameworkCore;

namespace Ordering;

public sealed record OrderSummary(string Number, string CustomerName, int LineCount, decimal Total);

public sealed class OrderQueries(OrdersDbContext db)
{
    // One SQL statement: the line count and the total are computed by the database, per order.
    public Task<List<OrderSummary>> SummariesAsync(CancellationToken cancellationToken = default) =>
        db.Orders
            .OrderBy(o => o.Number)
            .Select(o => new OrderSummary(
                o.Number,
                o.CustomerName,
                o.Lines.Count,
                o.Lines.Sum(l => l.Quantity * l.UnitPrice)))
            .ToListAsync(cancellationToken);

    // ILIKE is PostgreSQL's case-insensitive LIKE. A plain Contains would be case-sensitive there.
    public Task<List<string>> SearchByCustomerAsync(string term, CancellationToken cancellationToken = default) =>
        db.Orders
            .Where(o => EF.Functions.ILike(o.CustomerName, $"%{term}%"))
            .OrderBy(o => o.Number)
            .Select(o => o.Number)
            .ToListAsync(cancellationToken);

    // Deleted orders of the current tenant: only the soft-delete filter is switched off.
    public Task<List<string>> DeletedNumbersAsync(CancellationToken cancellationToken = default) =>
        db.Orders
            .IgnoreQueryFilters([OrdersDbContext.SoftDeleteFilter])
            .Where(o => o.IsDeleted)
            .OrderBy(o => o.Number)
            .Select(o => o.Number)
            .ToListAsync(cancellationToken);
}
