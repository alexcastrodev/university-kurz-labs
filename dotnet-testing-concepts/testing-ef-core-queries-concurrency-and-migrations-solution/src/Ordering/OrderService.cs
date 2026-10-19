using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Ordering;

public sealed class DuplicateOrderNumberException(string number, Exception inner)
    : Exception($"Order number {number} is already taken.", inner)
{
    public string Number { get; } = number;
}

public sealed record NewLine(string Sku, int Quantity, decimal UnitPrice);

public sealed class OrderService(OrdersDbContext db)
{
    public async Task<Order> PlaceAsync(string number, string customerName, IEnumerable<NewLine> lines,
        CancellationToken cancellationToken = default)
    {
        var order = new Order(db.TenantId, number, customerName);
        foreach (var line in lines)
        {
            order.AddLine(line.Sku, line.Quantity, line.UnitPrice);
        }

        db.Orders.Add(order);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: OrdersDbContext.UniqueOrderNumberIndex,
        })
        {
            // The index is the guarantee; the domain gets an error it can act on, not a provider exception.
            db.Entry(order).State = EntityState.Detached;
            throw new DuplicateOrderNumberException(number, e);
        }

        return order;
    }

    public async Task DeleteAsync(string number, CancellationToken cancellationToken = default)
    {
        var order = await db.Orders.SingleAsync(o => o.Number == number, cancellationToken);
        order.Delete();
        await db.SaveChangesAsync(cancellationToken);
    }
}
