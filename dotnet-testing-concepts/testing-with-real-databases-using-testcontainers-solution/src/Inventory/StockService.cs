using Microsoft.EntityFrameworkCore;

namespace Inventory;

public sealed class StockService(InventoryDbContext db)
{
    public async Task<Product> RegisterAsync(string sku, string name, CancellationToken cancellationToken = default)
    {
        var product = new Product(sku, name);
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task ReceiveAsync(string sku, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.SingleAsync(p => p.Sku == sku, cancellationToken);
        product.Receive(quantity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task ReserveAsync(string sku, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.SingleAsync(p => p.Sku == sku, cancellationToken);
        product.Reserve(quantity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<List<string>> LowStockSkusAsync(int threshold, CancellationToken cancellationToken = default) =>
        db.Products
            .Where(p => p.OnHand < threshold)
            .OrderBy(p => p.Sku)
            .Select(p => p.Sku)
            .ToListAsync(cancellationToken);
}
